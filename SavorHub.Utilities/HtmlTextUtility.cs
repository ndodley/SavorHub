using System.Net;
using System.Text.RegularExpressions;

namespace SavorHub.Utilities
{
    // Converts menu item descriptions between the HTML that TinyMCE stores and
    // the plain text shown in CSV exports, so exported files are readable in
    // Excel and can be uploaded again without losing paragraphs.
    public static class HtmlTextUtility
    {
        private static readonly Regex LineBreakTags = new(@"<\s*br\s*/?\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BlockEndTags = new(@"</\s*(p|div|li|h[1-6])\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex AnyTag = new(@"<[^>]+>", RegexOptions.Compiled);
        private static readonly Regex LooksLikeHtml = new(@"<\s*/?\s*[a-zA-Z][^>]*>", RegexOptions.Compiled);

        // "<p>Hot &amp; spicy</p><p>Extra sauce</p>"  ->  "Hot & spicy\nExtra sauce"
        public static string HtmlToPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var text = LineBreakTags.Replace(html, "\n");
            text = BlockEndTags.Replace(text, "\n");
            text = AnyTag.Replace(text, string.Empty);
            text = WebUtility.HtmlDecode(text).Replace(' ', ' ');

            return string.Join("\n", SplitLines(text));
        }

        // "Hot & spicy\nExtra sauce"  ->  "<p>Hot &amp; spicy</p><p>Extra sauce</p>"
        // Values that already contain HTML tags are kept as they are.
        public static string PlainTextToHtml(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if (LooksLikeHtml.IsMatch(text))
            {
                return text.Trim();
            }

            return string.Concat(SplitLines(text).Select(line => $"<p>{WebUtility.HtmlEncode(line)}</p>"));
        }

        private static IEnumerable<string> SplitLines(string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => line.Length > 0);
        }
    }
}
