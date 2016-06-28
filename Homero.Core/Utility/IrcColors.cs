namespace Homero.Core.Utility
{
    public enum ColorCode
    {
        White = 0, /**< White */
        Black = 1, /**< Black */
        DarkBlue = 2, /**< Dark blue */
        DarkGreen = 3, /**< Dark green */
        Red = 4, /**< Red */
        DarkRed = 5, /**< Dark red */
        DarkViolet = 6, /**< Dark violet */
        Orange = 7, /**< Orange */
        Yellow = 8, /**< Yellow */
        LightGreen = 9, /**< Light green */
        Cyan = 10, /**< Cornflower blue */
        LightCyan = 11, /**< Light blue */
        Blue = 12, /**< Blue */
        Violet = 13, /**< Violet */
        DarkGray = 14, /**< Dark gray */
        LightGray = 15 /**< Light gray */
    }

    public enum ControlCode
    {
        Bold = 0x02, /**< Bold */
        Color = 0x03, /**< Color */
        Italic = 0x09, /**< Italic */
        StrikeThrough = 0x13, /**< Strike-Through */
        Reset = 0x0f, /**< Reset */
        Underline = 0x15, /**< Underline */
        Underline2 = 0x1f, /**< Underline */
        Reverse = 0x16 /**< Reverse */
    }
}