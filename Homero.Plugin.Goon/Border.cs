using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homero.Client;
using Homero.EventArgs;
using Homero.Messages;
using Homero.Services;

namespace Homero.Plugin
{
    public class Border : IPlugin
    {
        private const int BORDER_MAX_WIDTH = 40;

        private List<string> WrapText(string text, int maxWidth)
        {
            List<string> wrappedText = new List<string>();
            List<string> words = text.Split(' ').ToList();
            StringBuilder currentLine = new StringBuilder();

            foreach (string word in words)
            {
                string mutableWord = word;

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
            int max = lines.Max(x => x.Length);
            int maxLine = max + 6;

            List<string> outputLines = new List<string>();

            BorderedLine headerBorder = new BorderedLine
            {
                Left = "  _.",
                Right = "._ ",
                Fill = '-'
            };
            BorderedLine standardBorder = new BorderedLine
            {
                Left = " | ",
                Right = " |",
                Fill = ' ',
            };
            BorderedLine footerBorder = new BorderedLine
            {
                Left = " |",
                Right = "|",
                Fill = '_'
            };
            BorderedLine baseBorder = new BorderedLine
            {
                Left = "|",
                Right = "|",
                Fill = '_'
            };

            outputLines.Add(headerBorder.FillToWidth(maxLine - 1)); // - 1 cause we want it to end 1 char early
            outputLines.Add(standardBorder.SurroundToWidth("RIP", max));

            outputLines.AddRange(lines.Select((line) => standardBorder.SurroundToWidth(line.ToUpper(), max)));

            outputLines.Add(footerBorder.FillToWidth(maxLine - 1));
            outputLines.Add(baseBorder.FillToWidth(maxLine));

            return outputLines;
        }

        private List<string> FormatTextToBread(List<string> lines)
        {
            int max = lines.Max(x => x.Length);
            int maxLine = max + 5;

            List<string> outputLines = new List<string>();

            BorderedLine headerBorder = new BorderedLine
            {
                Left = " .",
                Right = ". ",
                Fill = '-'
            };
            BorderedLine standardBorder = new BorderedLine
            {
                Left = "| ",
                Right = " |",
                Fill = ' ',
            };
            BorderedLine footerBorder = new BorderedLine
            {
                Left = "|",
                Right = "|",
                Fill = '_'
            };

            outputLines.Add(headerBorder.FillToWidth(maxLine - 1));
            outputLines.AddRange(lines.Select((line) => standardBorder.SurroundToWidth(line.ToUpper(), max)));
            outputLines.Add(footerBorder.FillToWidth(maxLine - 1));

            return outputLines;
        }

        public Border(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
        }

        public List<string> RegisteredTextCommands
        {
            get { return new List<string>() { "rip", "bread" }; }
        }

        public void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            IClient client = sender as IClient;
            string message = String.Join(" ", e.Command.Arguments);
            List<string> wrappedText = WrapText(message, BORDER_MAX_WIDTH);
            List<string> outputLines = new List<string>();

            if (e.Command.Command == "rip")
            {
                outputLines = FormatTextToHeadstone(wrappedText);
            }
            else if (e.Command.Command == "bread")
            {
                outputLines = FormatTextToBread(wrappedText);
            }

            if (client is DiscordClient)
            {
                string combinedText = String.Format("```{0}```", String.Join("\n", outputLines));
                client.ReplyTo(e.Command, combinedText);
            }
        }
        

        public void Shutdown()
        {
        }

        #region utility class

        private class BorderedLine
        {
            public string Left { get; set; }
            public string Right { get; set; }
            public char Fill { get; set; }

            public string FillToWidth(int width)
            {
                int toFill = width - Left.Length - Right.Length;
                return Left + new string(Fill, toFill) + Right;
            }

            public string SurroundToWidth(string text, int width)
            {
                int toFill = width - Left.Length - Right.Length - text.Length;
                return Left + PadToWidth(text, width) + Right;
            }

            private string PadToWidth(string text, int width)
            {
                int gapToFill = width - text.Length;
                return text.PadLeft(gapToFill / 2 + text.Length).PadRight(width);
            }
        }

        #endregion utility class
    }
}