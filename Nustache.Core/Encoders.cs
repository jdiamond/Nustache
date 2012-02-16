using System.Globalization;
using System.Text;

namespace Nustache.Core
{
    public static class Encoders
    {
        // Used with permission from http://www.west-wind.com/weblog/posts/2009/Feb/05/Html-and-Uri-String-Encoding-without-SystemWeb
        public static string HtmlEncode(string text)
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
                        if (text[i] > 159)
                        {
                            sb.Append("&#");
                            sb.Append(((int)text[i]).ToString(CultureInfo.InvariantCulture));
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