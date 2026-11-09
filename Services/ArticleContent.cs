using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace Bayan.Services
{
    /// <summary>
    /// Handles storage, sanitisation and presentation of article content.
    /// The rich-text editor produces HTML; everything saved is filtered through
    /// an allow-list so the stored markup can be rendered safely.
    /// </summary>
    public static class ArticleContent
    {
        private static readonly HashSet<string> AllowedTags = new(StringComparer.OrdinalIgnoreCase)
        {
            "p", "br", "h2", "h3", "h4", "strong", "b", "em", "i", "u", "s",
            "ul", "ol", "li", "blockquote", "pre", "code", "a", "img", "hr",
            "figure", "figcaption"
        };

        private static readonly HashSet<string> VoidTags = new(StringComparer.OrdinalIgnoreCase)
        {
            "br", "hr", "img"
        };

        private static readonly Regex DangerousBlocks = new(
            @"<\s*(script|style|iframe|object|embed|form|input|button|select|textarea|svg|math|link|meta|base|video|audio|source)\b[^>]*>.*?<\s*/\s*\1\s*>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex DangerousSingles = new(
            @"<\s*/?\s*(script|style|iframe|object|embed|form|input|button|select|textarea|svg|math|link|meta|base|video|audio|source)\b[^>]*>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex HtmlComment = new(
            @"<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex TagRegex = new(
            @"<\s*(/?)\s*([a-zA-Z][a-zA-Z0-9]*)((?:\s+[^<>]*?)?)\s*(/?)\s*>",
            RegexOptions.Compiled);

        private static readonly Regex AttributeRegex = new(
            @"([a-zA-Z_:][-a-zA-Z0-9_:.]*)\s*=\s*(?:""([^""]*)""|'([^']*)'|([^\s""'>`=]+))",
            RegexOptions.Compiled);

        private static readonly Regex TagPresence = new(
            @"<\s*/?\s*(p|br|h2|h3|h4|strong|em|ul|ol|li|blockquote|pre|code|a|img|figure)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex AnyTag = new(@"<[^>]+>", RegexOptions.Compiled);
        private static readonly Regex MultiWhitespace = new(@"[ \t]+", RegexOptions.Compiled);
        private static readonly Regex ManyBlankLines = new(@"\n{3,}", RegexOptions.Compiled);

        public static string Sanitize(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            var value = HtmlComment.Replace(html, string.Empty);
            value = DangerousBlocks.Replace(value, string.Empty);
            value = DangerousSingles.Replace(value, string.Empty);

            var builder = new StringBuilder(value.Length);
            var lastIndex = 0;

            foreach (Match match in TagRegex.Matches(value))
            {
                if (match.Index > lastIndex)
                    builder.Append(value, lastIndex, match.Index - lastIndex);

                lastIndex = match.Index + match.Length;

                var isClosing = match.Groups[1].Value == "/";
                var tag = match.Groups[2].Value.ToLowerInvariant();
                var rawAttributes = match.Groups[3].Value;

                if (!AllowedTags.Contains(tag))
                    continue;

                if (isClosing)
                {
                    if (!VoidTags.Contains(tag))
                        builder.Append("</").Append(tag).Append('>');
                    continue;
                }

                var attributes = BuildAttributes(tag, rawAttributes);
                builder.Append('<').Append(tag).Append(attributes);
                builder.Append(VoidTags.Contains(tag) ? " />" : ">");
            }

            if (lastIndex < value.Length)
                builder.Append(value, lastIndex, value.Length - lastIndex);

            var cleaned = builder.ToString().Trim();
            return string.IsNullOrWhiteSpace(cleaned) ? string.Empty : WrapPlainTextIfNeeded(cleaned);
        }

        public static string Render(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            if (TagPresence.IsMatch(content))
                return content.Trim();

            return WrapPlainTextIfNeeded(content);
        }

        public static string PlainText(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var text = AnyTag.Replace(content, " ");
            text = WebUtility.HtmlDecode(text);
            text = MultiWhitespace.Replace(text, " ");
            return text.Trim();
        }

        public static string Excerpt(string? content, int maxLength = 170)
        {
            var text = PlainText(content);
            if (text.Length <= maxLength)
                return text;

            var cut = text.LastIndexOf(' ', Math.Min(maxLength, text.Length - 1));
            if (cut < maxLength / 2)
                cut = maxLength;

            return text[..cut].TrimEnd(' ', '.', ',', ';', ':') + "…";
        }

        public static int ReadMinutes(string? content)
        {
            var text = PlainText(content);
            if (string.IsNullOrWhiteSpace(text))
                return 1;

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            return Math.Max(1, (int)Math.Ceiling(words / 200.0));
        }

        private static string BuildAttributes(string tag, string rawAttributes)
        {
            var allowed = tag switch
            {
                "a" => new[] { "href", "title" },
                "img" => new[] { "src", "alt", "title", "width", "height" },
                _ => Array.Empty<string>()
            };

            if (allowed.Length == 0 || string.IsNullOrWhiteSpace(rawAttributes))
                return string.Empty;

            var builder = new StringBuilder();

            foreach (Match match in AttributeRegex.Matches(rawAttributes))
            {
                var name = match.Groups[1].Value.ToLowerInvariant();
                if (!allowed.Contains(name))
                    continue;

                var value = match.Groups[2].Success ? match.Groups[2].Value
                    : match.Groups[3].Success ? match.Groups[3].Value
                    : match.Groups[4].Value;

                value = WebUtility.HtmlDecode(value).Trim();

                if (name == "href" && !UrlSafety.IsSafeLink(value))
                    continue;

                if (name == "src" && !UrlSafety.IsSafeImageSource(value))
                    continue;

                if ((name == "width" || name == "height") && !int.TryParse(value, out _))
                    continue;

                if (value.Length > 500)
                    value = value[..500];

                builder.Append(' ').Append(name).Append("=\"")
                    .Append(HtmlEncoder.Default.Encode(value)).Append('"');
            }

            return builder.ToString();
        }

        private static string WrapPlainTextIfNeeded(string content)
        {
            if (TagPresence.IsMatch(content))
                return content;

            var normalized = content.Replace("\r\n", "\n").Replace('\r', '\n');
            normalized = ManyBlankLines.Replace(normalized, "\n\n");

            var paragraphs = normalized
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(paragraph =>
                {
                    var encoded = HtmlEncoder.Default.Encode(paragraph.Trim());
                    return "<p>" + encoded.Replace("\n", "<br />") + "</p>";
                });

            return string.Concat(paragraphs);
        }
    }

    public static class UrlSafety
    {
        public static bool IsSafeLink(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.StartsWith('/') || value.StartsWith('#'))
                return !value.StartsWith("//", StringComparison.Ordinal);

            return Uri.TryCreate(value, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp
                       || uri.Scheme == Uri.UriSchemeHttps
                       || uri.Scheme == Uri.UriSchemeMailto);
        }

        public static bool IsSafeImageSource(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.StartsWith("/PostImages/", StringComparison.OrdinalIgnoreCase))
                return true;

            return Uri.TryCreate(value, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>Normalises a user-supplied website/social URL.</summary>
        public static string? NormalizeExternal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();
            if (!value.Contains("://", StringComparison.Ordinal))
                value = "https://" + value;

            return Uri.TryCreate(value, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                ? uri.ToString()
                : null;
        }
    }
}
