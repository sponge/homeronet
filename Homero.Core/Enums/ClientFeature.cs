using System;

namespace Homero.Core
{
    [Flags]
    public enum ClientFeature
    {
        Text = 0,
        Markdown = 1,
        AudioChat = 2,
        MediaAttachments = 4,
        UrlInlining = 8,
        ColorControlCodes = 16
    }
}