using BMBattleReport.Services.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace BMBattleReport.Services
{
    public class HtmlCleanUpService : IHtmlCleanUpService
    {
        private readonly List<string> _requiredPhrases = new() { "Battle in", "<script", "<table", "<span class=\"UID_", "Total casualties:", "hits" };
        private readonly Dictionary<string, string> _replacementValuePairs = new()
            {
                //Change a bunch of colors to make things stand out better and give the report a better look and feel
                { "#000032", "#DEDEEA"},
                { "#FFFFFF", "#000000"},
                { "color: white", "color: black"},
                { "#004000", "#116811"},
                { "#DDDDDD", "#FF0000"},
                { "#E0E0FF", "#AD6557"},
                { "#C0FFC0", "#00FF00"},

                //Change some characters for differences in the HTML and Wiki code (including trailing spaces etc)
                { "\\r\\n", ""},
                { "<Char", "<"},
                { "</Char", "<"},
                { "< ", "<"},
                { "> ", ">"},
                { "<>", ""},
                { "</FONT<", "</FONT><"},
                { "<p>", "<br>"},
                { "  ", " "},
            };

        public string CleanUpHTML(string sourceCode)
        {
            var updatedSource = RemoveUnusedHtml(sourceCode);

            return ReplaceValuePairs(updatedSource);
        }

        private string RemoveUnusedHtml(string source)
        {
            var updatedSource = RemoveUnusedBeginningHtml(source);

            return RemoveUnusedEndHtml(updatedSource);
        }

        private string RemoveUnusedBeginningHtml(string source)
        {
            var index = source.IndexOf(_requiredPhrases[0]);

            return source[index..];
        }

        private string RemoveUnusedEndHtml(string source)
        {
            var index = source.IndexOf(_requiredPhrases[1]);

            return source[..index];
        }

        private string ReplaceValuePairs(string source)
        {
            var stringBuilder = new StringBuilder(source);

            foreach (var phrase in _replacementValuePairs)
            {
                if (source.Contains(phrase.Key))
                {
                    stringBuilder = stringBuilder.Replace(phrase.Key, phrase.Value);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
