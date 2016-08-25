namespace Homero.Plugin.Goon
{
    internal class FortuneFile
    {
        public FortuneFile(string command, string path, bool stripNewLines, bool isMultiline)
        {
            Command = command;
            Path = path;
            StripNewLines = stripNewLines;
            IsMultiLine = isMultiline;
        }

        public string Command { get; set; }

        public string Path { get; set; }

        public bool StripNewLines { get; set; }

        public bool IsMultiLine { get; set; }
    }
}