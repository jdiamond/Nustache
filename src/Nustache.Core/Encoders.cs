using System.Globalization;
using System.Text;

namespace Nustache.Core
{
    // We can't reference System.Web in Client Profile environments so this
    // class contains a simple encoder, but it can be swapped out with
    // HttpUtility.HtmlEncode when used in non-Client Profile environments
    // (like when using Nustache in an MVC application).
    public static class Encoders
    {
        public delegate string HtmlEncoder(string text);

        public static HtmlEncoder HtmlEncode { get; set; }

        static Encoders()
        {
            HtmlEncode = DefaultHtmlEncode;
        }

        // Used with permission from http://www.west-wind.com/weblog/posts/2009/Feb/05/Html-and-Uri-String-Encoding-without-SystemWeb
        public static string DefaultHtmlEncode(string text)
        {
            if (text == null)
            {
                return null;
            }

            var sb = new StringBuilder(text.Length);

            var len = text.Length;

            for (var i = 0; i < len; i++)
            {
                switch (text[i])
                {
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    default:
                        var ch = text[i];
                        if (ch > 159)
                        {
                            sb.Append("&#");
                            if (char.IsHighSurrogate(ch) && (i + 1) < len) {
                                // convert surrogates to their decimal value
                                sb.Append(char.ConvertToUtf32(ch, text[i+1]));
                                i++;
                            } else {
                                sb.Append(((int)ch).ToString(CultureInfo.InvariantCulture));
                            }
                            sb.Append(";");
                        }
                        else
                        {
                            sb.Append(text[i]);
                        }
                        break;
                }
            }

            return sb.ToString();
        }
    }
}