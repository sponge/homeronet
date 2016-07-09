using Homero.Core.Client;
using Homero.Core.EventArgs;
using Homero.Core.Services;
using System.Collections.Generic;
using System.Text;

namespace Homero.Plugin.Converter
{
    public class Jab : IPlugin
    {
        public Jab(IMessageBroker broker)
        {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup()
        {
        }

        public void Shutdown()
        {
        }

        public List<string> RegisteredTextCommands { get; } = new List<string> { "jab", "bigjab", "script" };

        public string FormatScript(string str)
        {
            var sb = new StringBuilder();

            foreach (var c in str)
            {
                if (c > 64 && c < 91)
                {
                    sb.Append(char.ConvertFromUtf32(c + 120107));
                }
                else if (c > 96 && c < 123)
                {
                    sb.Append(char.ConvertFromUtf32(c + 119841));
                }
                else
                {
                    sb.Append(char.ConvertFromUtf32(c));
                }
            }

            return sb.ToString();
        }

        public string FormatJab(string str)
        {
            var sb = new StringBuilder();

            foreach (var c in str)
            {
                // handle irc color codes correctly
                if (c == 3)
                {
                    // TODO: handle irc color codes correctly
                    // if ord(c) == 3:
                    //    out += c
                    //    out += inp[i + 1]
                    //    if inp[i + 2].isdigit():
                    //        out += inp[i + 2]
                    //        e.next()
                    //    e.next()
                }
                // convert spaces into fullwidth spaces
                else if (c == 32)
                {
                    sb.Append('\u3000');
                }
                // pass special chars through
                else if (c <= 31)
                {
                    sb.Append(char.ConvertFromUtf32(c));
                }
                else if (c > 176)
                {
                }
                else
                {
                    sb.Append(char.ConvertFromUtf32(c + 65248));
                }
            }

            return sb.ToString();
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e)
        {
            var client = sender as IClient;

            if (e.Command.Arguments?.Count == 0)
            {
                return;
            }

            var str = string.Join(" ", e.Command.Arguments);

            if (e.Command.Command == "script")
            {
                var outStr = FormatScript(str);
                e.ReplyTarget.Send(str);
            }
            else if (e.Command.Command == "jab" || e.Command.Command == "bigjab")
            {
                var outStr = FormatJab(str);

                if (e.Command.Command == "jab")
                {
                    e.ReplyTarget.Send(outStr);
                }
                else
                {
                    var fancy = "ஜ۩۞۩ஜ";

                    var padLen = (outStr.Length * 2 - fancy.Length) / 2 - 1;

                    var spaces = "";
                    if (padLen < 0)
                    {
                        outStr = outStr.PadLeft(fancy.Length / 2 + 1).PadRight(fancy.Length);
                    }
                    else
                    {
                        spaces = "".PadLeft(padLen, '\u25AC');
                    }

                    fancy = $"{spaces}{fancy}{spaces}";

                    outStr = $"{fancy}\n{outStr}\n{fancy}";

                    if (client?.MarkdownSupported == true)
                    {
                        outStr = $"```{outStr}```";
                    }

                    e.ReplyTarget.Send(outStr);
                }
            }
        }
    }
}