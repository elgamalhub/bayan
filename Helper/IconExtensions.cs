using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudyWithChatgpt.Helper
{
    /// <summary>
    /// Small inline SVG icon set. Avoids external icon CDNs and keeps the
    /// interface fully self-contained.
    /// </summary>
    public static class SvgIcons
    {
        private static readonly Dictionary<string, string> Paths = new(StringComparer.OrdinalIgnoreCase)
        {
            ["logo"] = "<path d=\"M12 6.2C9.9 4.8 7.1 4.4 4 5.2v13.6c3.1-.8 5.9-.4 8 1 2.1-1.4 4.9-1.8 8-1V5.2c-3.1-.8-5.9-.4-8 1Z\"/><path d=\"M12 6.2v13.6\"/><path d=\"M7.2 8.7c1.1-.1 2.1.1 3.1.5M7.2 11.6c1.1-.1 2.1.1 3.1.5M16.8 8.7c-1.1-.1-2.1.1-3.1.5M16.8 11.6c-1.1-.1-2.1.1-3.1.5\"/>",
            ["search"] = "<circle cx=\"11\" cy=\"11\" r=\"7\"/><path d=\"m20 20-3.6-3.6\"/>",
            ["sun"] = "<circle cx=\"12\" cy=\"12\" r=\"4\"/><path d=\"M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4\"/>",
            ["moon"] = "<path d=\"M20.5 14.3A8.5 8.5 0 0 1 9.7 3.5a8.5 8.5 0 1 0 10.8 10.8Z\"/>",
            ["globe"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"M3 12h18M12 3c2.5 2.5 3.8 5.6 3.8 9S14.5 18.5 12 21c-2.5-2.5-3.8-5.6-3.8-9S9.5 5.5 12 3Z\"/>",
            ["menu"] = "<path d=\"M4 7h16M4 12h16M4 17h16\"/>",
            ["x"] = "<path d=\"m6 6 12 12M18 6 6 18\"/>",
            ["user"] = "<circle cx=\"12\" cy=\"8\" r=\"3.5\"/><path d=\"M5 20c.8-3.5 3.5-5.5 7-5.5s6.2 2 7 5.5\"/>",
            ["users"] = "<circle cx=\"9\" cy=\"8\" r=\"3\"/><path d=\"M3 20c.6-3.4 2.9-5.4 6-5.4s5.4 2 6 5.4M16 5.5a3 3 0 0 1 0 5.8M17 14.4c2.4.4 3.8 2.3 4 5.6\"/>",
            ["pen"] = "<path d=\"M4 20h4l10.5-10.5a2.1 2.1 0 0 0-3-3L5 17v3Z\"/><path d=\"m14 7 3 3\"/>",
            ["trash"] = "<path d=\"M4 7h16M9 7V5h6v2M6 7l1 13h10l1-13M10 11v5M14 11v5\"/>",
            ["heart"] = "<path d=\"M12 20.5S4 15.5 4 9.8A4.3 4.3 0 0 1 12 7a4.3 4.3 0 0 1 8 2.8c0 5.7-8 10.7-8 10.7Z\"/>",
            ["heart-filled"] = "<path fill=\"currentColor\" stroke=\"none\" d=\"M12 20.5S4 15.5 4 9.8A4.3 4.3 0 0 1 12 7a4.3 4.3 0 0 1 8 2.8c0 5.7-8 10.7-8 10.7Z\"/>",
            ["message"] = "<path d=\"M20 15a3 3 0 0 1-3 3H9l-4 3v-3.6A3 3 0 0 1 4 15V7a3 3 0 0 1 3-3h10a3 3 0 0 1 3 3v8Z\"/>",
            ["calendar"] = "<rect x=\"4\" y=\"5\" width=\"16\" height=\"15\" rx=\"2\"/><path d=\"M8 3v4M16 3v4M4 10h16\"/>",
            ["clock"] = "<circle cx=\"12\" cy=\"12\" r=\"8.5\"/><path d=\"M12 7.5V12l3 2\"/>",
            ["tag"] = "<path d=\"M4 5v6.2L13.8 21 21 13.8 11.2 4H5a1 1 0 0 0-1 1Z\"/><circle cx=\"8\" cy=\"8\" r=\"1.2\" fill=\"currentColor\" stroke=\"none\"/>",
            ["eye"] = "<path d=\"M2.5 12S6 6.5 12 6.5 21.5 12 21.5 12 18 17.5 12 17.5 2.5 12 2.5 12Z\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/>",
            ["eye-off"] = "<path d=\"M3 3l18 18M10.6 6.7A9.9 9.9 0 0 1 12 6.5c6 0 9.5 5.5 9.5 5.5a17 17 0 0 1-3.1 3.6M6.2 6.8C3.7 8.6 2.5 12 2.5 12S6 17.5 12 17.5a9.8 9.8 0 0 0 4.2-.9\"/>",
            ["plus"] = "<path d=\"M12 5v14M5 12h14\"/>",
            ["image"] = "<rect x=\"3.5\" y=\"4.5\" width=\"17\" height=\"15\" rx=\"2\"/><circle cx=\"9\" cy=\"10\" r=\"1.6\"/><path d=\"m4 17 5-5 4 4 3-3 4 4\"/>",
            ["arrow-right"] = "<path d=\"M4 12h16M14 6l6 6-6 6\"/>",
            ["arrow-left"] = "<path d=\"M20 12H4M10 6l-6 6 6 6\"/>",
            ["arrow-up"] = "<path d=\"M12 20V4M6 10l6-6 6 6\"/>",
            ["chevron-down"] = "<path d=\"m6 9 6 6 6-6\"/>",
            ["chevron-left"] = "<path d=\"m15 5-7 7 7 7\"/>",
            ["chevron-right"] = "<path d=\"m9 5 7 7-7 7\"/>",
            ["check"] = "<path d=\"m5 12 4.5 4.5L19 7\"/>",
            ["alert"] = "<path d=\"M12 4 2.8 20h18.4L12 4Z\"/><path d=\"M12 9v5\"/><circle cx=\"12\" cy=\"17\" r=\".9\" fill=\"currentColor\" stroke=\"none\"/>",
            ["info"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"M12 11v6\"/><circle cx=\"12\" cy=\"7.6\" r=\".9\" fill=\"currentColor\" stroke=\"none\"/>",
            ["logout"] = "<path d=\"M14 4h5a1 1 0 0 1 1 1v14a1 1 0 0 1-1 1h-5M10 8l-4 4 4 4M6 12h11\"/>",
            ["settings"] = "<circle cx=\"12\" cy=\"12\" r=\"3\"/><path d=\"M19.4 15a1.7 1.7 0 0 0 .3 1.9l.1.1-2.8 2.8-.1-.1a1.7 1.7 0 0 0-1.9-.3 1.7 1.7 0 0 0-1 1.5V21h-4v-.1a1.7 1.7 0 0 0-1-1.5 1.7 1.7 0 0 0-1.9.3l-.1.1L4.2 17l.1-.1a1.7 1.7 0 0 0 .3-1.9 1.7 1.7 0 0 0-1.5-1H3v-4h.1a1.7 1.7 0 0 0 1.5-1 1.7 1.7 0 0 0-.3-1.9L4.2 7 7 4.2l.1.1a1.7 1.7 0 0 0 1.9.3 1.7 1.7 0 0 0 1-1.5V3h4v.1a1.7 1.7 0 0 0 1 1.5 1.7 1.7 0 0 0 1.9-.3l.1-.1L19.8 7l-.1.1a1.7 1.7 0 0 0-.3 1.9 1.7 1.7 0 0 0 1.5 1h.1v4h-.1a1.7 1.7 0 0 0-1.5 1Z\"/>",
            ["book-open"] = "<path d=\"M12 6.5c-2.5-1.4-5.4-1.6-8-1v14c2.6-.6 5.5-.4 8 1 2.5-1.4 5.4-1.6 8-1v-14c-2.6-.6-5.5-.4-8 1Z\"/><path d=\"M12 6.5v14\"/>",
            ["shield"] = "<path d=\"M12 3 5 6v5.5c0 4.4 2.9 8.1 7 9.5 4.1-1.4 7-5.1 7-9.5V6l-7-3Z\"/><path d=\"m9 12 2 2 4-4\"/>",
            ["mail"] = "<rect x=\"3\" y=\"5\" width=\"18\" height=\"14\" rx=\"2\"/><path d=\"m4 7 8 6 8-6\"/>",
            ["linkedin"] = "<path fill=\"currentColor\" stroke=\"none\" d=\"M6.9 8.7H4V20h2.9V8.7ZM5.4 4a1.7 1.7 0 1 0 0 3.4 1.7 1.7 0 0 0 0-3.4ZM20 13.6c0-3.2-1.7-4.6-4-4.6-1.8 0-2.6 1-3 1.7V8.7h-3V20h3v-6.1c0-1.5.6-2.4 1.9-2.4 1.2 0 1.8.8 1.8 2.4V20h3v-6.4Z\"/>",
            ["external"] = "<path d=\"M14 4h6v6M20 4l-8 8M19 14v4a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V7a2 2 0 0 1 2-2h4\"/>",
            ["upload"] = "<path d=\"M12 16V4M7 9l5-5 5 5M4 17v2a1 1 0 0 0 1 1h14a1 1 0 0 0 1-1v-2\"/>",
            ["share"] = "<circle cx=\"6\" cy=\"12\" r=\"2.5\"/><circle cx=\"17\" cy=\"6\" r=\"2.5\"/><circle cx=\"17\" cy=\"18\" r=\"2.5\"/><path d=\"m8.3 10.9 6.4-3.6M8.3 13.1l6.4 3.6\"/>",
            ["filter"] = "<path d=\"M4 6h16M7 12h10M10 18h4\"/>",
            ["grid"] = "<rect x=\"4\" y=\"4\" width=\"6\" height=\"6\" rx=\"1\"/><rect x=\"14\" y=\"4\" width=\"6\" height=\"6\" rx=\"1\"/><rect x=\"4\" y=\"14\" width=\"6\" height=\"6\" rx=\"1\"/><rect x=\"14\" y=\"14\" width=\"6\" height=\"6\" rx=\"1\"/>",
            ["lock"] = "<rect x=\"5\" y=\"10\" width=\"14\" height=\"10\" rx=\"2\"/><path d=\"M8 10V7a4 4 0 0 1 8 0v3\"/>",
            ["bold"] = "<path d=\"M7 4h6a4 4 0 0 1 0 8H7V4Zm0 8h7a4 4 0 0 1 0 8H7v-8Z\"/>",
            ["italic"] = "<path d=\"M10 4h8M6 20h8M14 4 10 20\"/>",
            ["underline"] = "<path d=\"M7 4v7a5 5 0 0 0 10 0V4M5 21h14\"/>",
            ["strike"] = "<path d=\"M6 8a4 4 0 0 1 4-4h4a4 4 0 0 1 3.5 2M18 16a4 4 0 0 1-4 4h-4a4 4 0 0 1-3.5-2M4 12h16\"/>",
            ["h2"] = "<path d=\"M5 5v14M13 5v14M5 12h8\"/><path d=\"M17 9a2 2 0 1 1 3.2 1.6L17 15h4\"/>",
            ["h3"] = "<path d=\"M5 5v14M13 5v14M5 12h8\"/><path d=\"M17 9a2 2 0 0 1 3.4 1.4A2 2 0 0 1 18.5 13a2 2 0 0 1 2 2.2A2.1 2.1 0 0 1 18 17.2\"/>",
            ["paragraph"] = "<path d=\"M13 4v16M17 4v16M8 4h9M8 4a4 4 0 0 0 0 8h5\"/>",
            ["list"] = "<path d=\"M9 6h11M9 12h11M9 18h11\"/><circle cx=\"5\" cy=\"6\" r=\"1\" fill=\"currentColor\" stroke=\"none\"/><circle cx=\"5\" cy=\"12\" r=\"1\" fill=\"currentColor\" stroke=\"none\"/><circle cx=\"5\" cy=\"18\" r=\"1\" fill=\"currentColor\" stroke=\"none\"/>",
            ["list-ordered"] = "<path d=\"M10 6h10M10 12h10M10 18h10\"/><path d=\"M4 5h1v4M3.5 9H6M3.5 12.6A1.2 1.2 0 0 1 6 13a1.2 1.2 0 0 1-2.3.6M3.5 15h2.5l-2 3H6\" stroke-width=\"1.3\"/>",
            ["quote"] = "<path d=\"M5 11h4v7H4v-4a9 9 0 0 1 4-7M15 11h4v7h-5v-4a9 9 0 0 1 4-7\"/>",
            ["code"] = "<path d=\"m8 8-5 4 5 4M16 8l5 4-5 4M14 4l-4 16\"/>",
            ["link"] = "<path d=\"M10 13a4 4 0 0 0 5.7 0l2.6-2.6a4 4 0 0 0-5.7-5.7L11.4 6\"/><path d=\"M14 11a4 4 0 0 0-5.7 0l-2.6 2.6a4 4 0 0 0 5.7 5.7l1.2-1.2\"/>",
            ["undo"] = "<path d=\"M9 7 4 12l5 5M4 12h10a6 6 0 0 1 0 12\"/>",
            ["redo"] = "<path d=\"m15 7 5 5-5 5M20 12H10a6 6 0 0 0 0 12\"/>",
            ["clear-format"] = "<path d=\"M4 20h16M7 4h10M12 4v8M8.5 16l7-7\"/>",
            ["more"] = "<circle cx=\"5\" cy=\"12\" r=\"1.3\" fill=\"currentColor\" stroke=\"none\"/><circle cx=\"12\" cy=\"12\" r=\"1.3\" fill=\"currentColor\" stroke=\"none\"/><circle cx=\"19\" cy=\"12\" r=\"1.3\" fill=\"currentColor\" stroke=\"none\"/>",
            ["type"] = "<path d=\"M5 5h14M12 5v14M8 19h8\"/>"
        };

        public static string Render(string name, string? cssClass = null, int size = 20, string? title = null)
        {
            var inner = Paths.TryGetValue(name, out var svg) ? svg : Paths["info"];
            var builder = new StringBuilder();
            builder.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" width=\"").Append(size)
                .Append("\" height=\"").Append(size)
                .Append("\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.75\" stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"")
                .Append(string.IsNullOrEmpty(title) ? "true" : "false")
                .Append("\" class=\"icon")
                .Append(string.IsNullOrWhiteSpace(cssClass) ? string.Empty : " " + cssClass)
                .Append("\" focusable=\"false\">");

            if (!string.IsNullOrEmpty(title))
                builder.Append("<title>").Append(System.Net.WebUtility.HtmlEncode(title)).Append("</title>");

            builder.Append(inner).Append("</svg>");
            return builder.ToString();
        }
    }

    public static class HtmlIconExtensions
    {
        public static IHtmlContent Icon(this IHtmlHelper html, string name, string? cssClass = null, int size = 20, string? title = null)
        {
            return new HtmlString(SvgIcons.Render(name, cssClass, size, title));
        }

        public static IHtmlContent Avatar(this IHtmlHelper html, string? imageUrl, string initials, string alt, string extraClass = "")
        {
            var css = string.IsNullOrWhiteSpace(extraClass) ? "avatar" : $"avatar {extraClass}";
            var safeAlt = System.Net.WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(alt) ? "Member" : alt);

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                return new HtmlString(
                    $"<span class=\"{css}\"><img src=\"{System.Net.WebUtility.HtmlEncode(imageUrl)}\" alt=\"{safeAlt}\" loading=\"lazy\" /></span>");
            }

            var safeInitials = System.Net.WebUtility.HtmlEncode(
                string.IsNullOrWhiteSpace(initials) ? "?" : initials);

            return new HtmlString(
                $"<span class=\"{css} avatar-initials\" role=\"img\" aria-label=\"{safeAlt}\">{safeInitials}</span>");
        }
    }
}
