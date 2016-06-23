using System;
using System.Collections.Generic;
using Homero.Client;
using Homero.EventArgs;
using Homero.Services;
using System.Text;

namespace Homero.Plugin.Converter {
    public class Jab : IPlugin {
        private List<string> _registeredCommands = new List<string>() { "jab", "bigjab", "script" };

        public Jab(IMessageBroker broker) {
            broker.CommandReceived += BrokerOnCommandReceived;
        }

        public void Startup() {
        }

        public void Shutdown() {
        }

        public List<string> RegisteredTextCommands {
            get { return _registeredCommands; }
        }

        public string FormatScript(string str) {
            StringBuilder sb = new StringBuilder();

            foreach (char c in str) {
                if (c > 64 && c < 91) {
                    sb.Append(Char.ConvertFromUtf32(c + 120107));
                }
                else if (c > 96 && c < 123) {
                    sb.Append(Char.ConvertFromUtf32(c + 119841));
                }
                else {
                    sb.Append(Char.ConvertFromUtf32(c));
                }
            }

            return sb.ToString();
        }

        public string FormatJab(string str) {
            StringBuilder sb = new StringBuilder();

            foreach (char c in str) {
                // handle irc color codes correctly
                if (c == 3) {
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
                else if (c == 32) {
                    sb.Append('\u3000');
                }
                // pass special chars through
                else if (c <= 31) {
                    sb.Append(Char.ConvertFromUtf32(c));
                }
                else if (c > 176) {
                    continue;
                }
                else {
                    sb.Append(Char.ConvertFromUtf32(c + 65248));
                }
            }

            return sb.ToString();
        }

        private void BrokerOnCommandReceived(object sender, CommandReceivedEventArgs e) {
            IClient client = sender as IClient;

            if (e.Command.Arguments?.Count == 0) {
                return;
            }

            var str = String.Join(" ", e.Command.Arguments);

            if (e.Command.Command == "script") {
                var outStr = FormatScript(str);
                client?.ReplyTo(e.Command, str);
            }
            else if (e.Command.Command == "jab" || e.Command.Command == "bigjab") {

                var outStr = FormatJab(str);

                if (e.Command.Command == "jab") {
                    client?.ReplyTo(e.Command, outStr);
                }
                else {
                    var fancy = "\u0B9C\u06E9\u06DE\u06E9\u0B9C";

                    var padLen = Math.Max((outStr.Length * 2 - fancy.Length) / 2 - 1, 0);

                    var spaces = "".PadLeft(padLen, '\u25AC');

                    fancy = $"{spaces}{fancy}{spaces}";

                    outStr = $"{fancy}\n{outStr}\n{fancy}";

                    if (client?.MarkdownSupported == true) {
                        outStr = $"```{outStr}```";
                    }

                    client?.ReplyTo(e.Command, outStr);
                }
            }
        }

    }
}
