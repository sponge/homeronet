using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homero.Plugin.Comic
{
    public class ComicRenderer
    {
        public void DrawComic(Comic comic, SKCanvas canvas, int width, int height)
        {
            PanelRenderer pre = new PanelRenderer();

            foreach (var item in comic.Panels.Select((panel, index) => new { index, panel }))
            {
                int startY = item.index * 300;
                canvas.SetMatrix(SKMatrix.MakeTranslation(0, startY));
                if (item.panel.IsTitle)
                {
                    pre.RenderTitle(item.panel.Messages.FirstOrDefault()?.Message, comic, canvas, width, 300);
                }
                else
                {
                    pre.RenderPanel(item.panel, comic, canvas, width, 300);
                }
            }
        }
    }

    public class PanelRenderer
    {
        private const int TEXT_SIZE = 18;
        private const int TEXT_PADDING = 10;

        private enum ImagePosition { Right, Left };

        private SKTypeface _typeFace = SKTypeface.FromFamilyName("Comic Sans MS");
        private SKColor _textFg = SKColors.White;
        private SKColor _textBg = SKColors.Black;

        public void RenderPanel(ComicPanel panel, Comic comic, SKCanvas canvas, int width, int height)
        {
            RenderBackground(comic, canvas, width, height);

            ComicMessage leftMsg = panel.Messages.First();
            ComicMessage rightMsg = panel.Messages.Last();
            
            // paint text
            using (SKPaint paint = new SKPaint())
            {
                paint.Typeface = _typeFace;
                paint.TextSize = TEXT_SIZE;

                SKRect bounds = new SKRect();
                paint.MeasureText("A", ref bounds);

                // paint left message
                DrawStrokedText(leftMsg.Message, TEXT_PADDING, -bounds.Top + TEXT_PADDING, SKTextAlign.Left, canvas);

                if (panel.Messages.Count == 2)
                {
                    // why -16? who knows. thanks skia.
                    DrawStrokedText(rightMsg.Message, width - 16 - TEXT_PADDING, -bounds.Top + TEXT_PADDING, SKTextAlign.Right, canvas);
                }
            }

            SKBitmap leftImg = SKBitmap.Decode(comic.Mappings.Get(leftMsg.User));
            DrawUserBitmap(leftImg, width, height, ImagePosition.Left, canvas);

            if (panel.Messages.Count == 2)
            {
                SKBitmap rightImg = SKBitmap.Decode(comic.Mappings.Get(rightMsg.User));
                DrawUserBitmap(rightImg, width, height, ImagePosition.Right, canvas);
            }
        }

        public void RenderTitle(string title, Comic comic, SKCanvas canvas, int width, int height)
        {
            RenderBackground(comic, canvas, width, height);

            using (SKPaint paint = new SKPaint())
            {
                paint.Typeface = _typeFace;
                paint.TextSize = TEXT_SIZE;
                DrawStrokedText(title, width/2, height/2, SKTextAlign.Center, canvas);
            }
        }

        public void RenderBackground(Comic comic, SKCanvas canvas, int width, int height)
        {
            // paint bg
            SKBitmap background = SKBitmap.Decode(comic.Background);
            canvas.DrawBitmap(background, new SKRect(0, 0, width, height));

            // paint bottom bar
            using (SKPaint paint = new SKPaint())
            {
                canvas.DrawRect(new SKRect(0, height - 5, width, height), paint);
            }
        }

        private void DrawStrokedText(string text, float x, float y, SKTextAlign alignment, SKCanvas canvas)
        {
            using (SKPaint textPaint = new SKPaint())
            using (SKPaint strokePaint = new SKPaint())
            {
                textPaint.Typeface = _typeFace;
                textPaint.TextSize = TEXT_SIZE;
                textPaint.Color = _textFg;
                textPaint.IsAntialias = true;
                textPaint.TextAlign = alignment;

                strokePaint.Typeface = _typeFace;
                strokePaint.TextSize = TEXT_SIZE;
                strokePaint.Color = _textBg;
                strokePaint.IsAntialias = true;
                strokePaint.IsStroke = true;
                strokePaint.StrokeWidth = 3.6f;
                strokePaint.TextAlign = alignment;
                strokePaint.StrokeJoin = SKStrokeJoin.Bevel;

                canvas.DrawText(text, x, y, strokePaint);
                canvas.DrawText(text, x, y, textPaint);
            }
        }

        private void DrawUserBitmap(SKBitmap bitmap, int width, int height, ImagePosition position, SKCanvas canvas)
        {
            using (SKPaint paint = new SKPaint())
            {
                float wScale = Math.Min(bitmap.Width, width / 2f) / bitmap.Width;
                float hScale = Math.Min(bitmap.Height, height * 0.75f) / bitmap.Height;
                float scale = Math.Min(wScale, hScale);

                SKMatrix scaleMatrix = new SKMatrix();
                scaleMatrix.SetScaleTranslate(scale, scale, 0, height - bitmap.Height * scale);

                paint.IsAntialias = true;

                if (position == ImagePosition.Left)
                {
                    paint.ImageFilter = SKImageFilter.CreateMatrix(scaleMatrix, SKFilterQuality.High);
                    canvas.DrawBitmap(bitmap, 0, 0, paint);
                }
                else
                {
                    scaleMatrix.TransX = width - 16;
                    scaleMatrix.ScaleX = -scaleMatrix.ScaleX;
                    paint.ImageFilter = SKImageFilter.CreateMatrix(scaleMatrix, SKFilterQuality.High);
                    canvas.DrawBitmap(bitmap, 0, 0, paint);
                }
            }
        }
    }
}
