using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Primitives;

namespace CalendarNotificationBot.Domain.Extensions
{
    public static class StringExtensions
    {
        private static readonly List<(Regex Pattern, string Replacement)> ReplacementDictionary =
        [
            (new(@"\[B\](.+?)\[/B\]", RegexOptions.Compiled), "<b>$1</b>"),
            (new(@"\[I\](.+?)\[/I\]", RegexOptions.Compiled), "<i>$1</i>"),
            (new(@"\[U\](.+?)\[/U\]", RegexOptions.Compiled), "<u>$1</u>"),
            (new(@"\[S\](.+?)\[/S\]", RegexOptions.Compiled), "<strike>$1</strike>"),
            (new(@"\[URL=(.+?)\](.+?)\[/URL\]", RegexOptions.Compiled), "<a href=\"$1\">$2</a>"),
            (new(@"\[IMG\](.+?)\[/IMG\]", RegexOptions.Compiled), "<a href=\"$1\">image link</a>"),
            (new(@"\[[^\]]+\]", RegexOptions.Compiled), ""),
            (new(@"\\n", RegexOptions.Compiled), "<br />")
        ];

        /// <summary>
        /// Escape strings for Telegram markdown.
        /// </summary>
        public static string EscapeStringForMarkdown(this string str)
        {
            return Regex.Replace(str, @"[_\*\[\]\(\)~`>#\+-=\|{}\.!]", m => "\\" + m.Groups[0].Value);
        }

        /// <summary>
        /// Escape strings for HTML.
        /// </summary>
        public static string EscapeStringForHtml(this string bbCode)
        {
            return ReplacementDictionary.Aggregate(
                bbCode,
                (current, regex) => regex.Pattern.Replace(current, regex.Replacement));
        }

        /// <summary>
        /// Convert links to Telegram markdown.
        /// </summary>
        public static string ConvertLinks(this string str)
        {
            var link = Regex.Replace(
                str,
                "(http|ftp|https):\\/\\/([\\w_-]+(?:(?:\\.[\\w_-]+)+))([\\w.,@?^=%&:\\/~+#-]*[\\w@?^=%&\\/~+#-])",
                m => $"[link]({m.Groups.ToString() ?? string.Empty})");
            return Regex.Replace(str, @"[_\*\[\]\(\)~`>#\+-=\|{}\.!]", m => "\\" + m.Groups[0].Value);
        }
    }
}