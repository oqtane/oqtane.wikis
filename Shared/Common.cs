using System.IO;
using System.Text.RegularExpressions;

namespace Oqtane.Wiki.Shared
{
    public static class Common
    {
        public static string FormatSlug(string title)
        {
            string slug = "";
            for (int i = 0; i < title.Length; i++)
            {
                var character = title.ToLower()[i];
                int ascii = (int)character;
                if ((ascii >= (int)'a' && ascii <= (int)'z') || (ascii >= (int)'0' && ascii <= (int)'9'))
                {
                    slug += character;
                }
                else
                {
                    if (character != '\'' && (i < title.Length - 1) && slug.Length > 0 && slug[slug.Length - 1] != '-')
                    {
                        slug += "-";
                    }
                }

            }
            return slug;
        }

        public static string CreateSummary(string content, int length, string search)
        {
            content = Regex.Replace(content, "<[a-zA-Z/].*?>", "");

            if (!string.IsNullOrEmpty(search))
            {
                var index = content.IndexOf(search, System.StringComparison.OrdinalIgnoreCase);
                index = index - (length / 2);
                index = (index < 0) ? 0 : index;
                content = content.Substring(index);
                content = content.Replace(search, $"<b><u>{search}</u></b>", System.StringComparison.OrdinalIgnoreCase);
            }

            if (content.Length <= length)
            {
                return content.Substring(0, content.Length);
            }
            else
            {
                return content.Substring(0, length) + "...";
            }
        }
    }
}
