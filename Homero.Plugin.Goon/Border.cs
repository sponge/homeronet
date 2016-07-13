using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Homero.Core.Interface;

namespace Homero.Plugin
{
    public class Border : IPlugin
    {
        private const int BORDER_MAX_WIDTH = 40;

        public Border(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
        }

        public List<string> RegisteredTextCommands
        {
            get { return new List<string> { "rip", "bread" }; }
        }

        public void Shutdown()
        {
        }

        private List<string> WrapText(string text, int maxWidth)
        {
            var wrappedText = new List<string>();
            var words = text.Split(' ').ToList();
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                var mutableWord = word;

                // if we will be too wide, add the line to the output list and start a new one
                if (currentLine.Length + word.Length + 1 > maxWidth)
                {
                    if (currentLine.Length > 0)
                    {
                        wrappedText.Add(currentLine.ToString());
                        currentLine.Clear();
                    }

                    while (mutableWord.Length > maxWidth)
                    {
                        // uh oh this word is huge, we need to break it apart
                        wrappedText.Add(mutableWord.Substring(0, maxWidth - 1));
                        mutableWord = mutableWord.Substring(maxWidth - 1);
                    }
                }

                currentLine.Append(" " + mutableWord);
            }

            if (currentLine.Length > 0)
            {
                wrappedText.Add(currentLine.ToString());
            }

            return wrappedText;
        }

        private List<string> FormatTextToHeadstone(List<string> lines)
        {
            var max = lines.Max(x => x.Length);
            var maxLine = max + 6;

            var outputLines = new List<string>();

            var headerBorder = new BorderedLine
            {
                Left = "  _.",
                Right = "._ ",
                Fill = '-'
            };
            var standardBorder = new BorderedLine
            {
                Left = " | ",
                Right = " |",
                Fill = ' '
            };
            var footerBorder = new BorderedLine
            {
                Left = " |",
                Right = "|",
                Fill = '_'
            };
            var baseBorder = new BorderedLine
            {
                Left = "|",
                Right = "|",
                Fill = '_'
            };

            outputLines.Add(headerBorder.FillToWidth(maxLine - 1)); // - 1 cause we want it to end 1 char early
            outputLines.Add(standardBorder.SurroundToWidth("RIP", max));

            outputLines.AddRange(lines.Select(line => standardBorder.SurroundToWidth(line.ToUpper(), max)));

            outputLines.Add(footerBorder.FillToWidth(maxLine - 1));
            outputLines.Add(baseBorder.FillToWidth(maxLine));

            return outputLines;
        }

        private List<string> FormatTextToBread(List<string> lines)
        {
            var max = lines.Max(x => x.Length);
            var maxLine = max + 5;

            var outputLines = new List<string>();

            var headerBorder = new BorderedLine
            {
                Left = " .",
                Right = ". ",
                Fill = '-'
            };
            var standardBorder = new BorderedLine
            {
                Left = "| ",
                Right = " |",
                Fill = ' '
            };
            var footerBorder = new BorderedLine
            {
                Left = "|",
                Right = "|",
                Fill = '_'
            };

            outputLines.Add(headerBorder.FillToWidth(maxLine - 1));
            outputLines.AddRange(lines.Select(line => standardBorder.SurroundToWidth(line.ToUpper(), max)));
            outputLines.Add(footerBorder.FillToWidth(maxLine - 1));

            return outputLines;
        }

        public void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;
            if (e.Command.Arguments.Count == 0)
            {
                return;
            }
            var message = string.Join(" ", e.Command.Arguments);
            var wrappedText = WrapText(message, BORDER_MAX_WIDTH);
            var outputLines = new List<string>();

            if (e.Command.Command == "rip")
            {
                outputLines = FormatTextToHeadstone(wrappedText);
            }
            else if (e.Command.Command == "bread")
            {
                outputLines = FormatTextToBread(wrappedText);
            }

            if (client?.MarkdownSupported == true)
            {
                var combinedText = string.Format("```{0}```", string.Join("\n", outputLines));
                e.ReplyTarget.Send(combinedText);
            }
            else
            {
                e.ReplyTarget.Send(string.Join("\n", outputLines));
            }
        }

        #region utility class

        private class BorderedLine
        {
            public string Left { get; set; }

            public string Right { get; set; }

            public char Fill { get; set; }

            public string FillToWidth(int width)
            {
                var toFill = width - Left.Length - Right.Length;
                return Left + new string(Fill, toFill) + Right;
            }

            public string SurroundToWidth(string text, int width)
            {
                var toFill = width - Left.Length - Right.Length - text.Length;
                return Left + PadToWidth(text, width) + Right;
            }

            private string PadToWidth(string text, int width)
            {
                var gapToFill = width - text.Length;
                return text.PadLeft(gapToFill / 2 + text.Length).PadRight(width);
            }
        }

        #endregion utility class
    }
}