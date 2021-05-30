using BMBattleReport.Services.Interfaces;

namespace BMBattleReport.Services
{
    public class HelperService : IHelperService
    {
        public string GetSubstringMiddle(string source, string indexBegin, string indexEnd, bool includeIndexEnd = false)
        {
            var firstIndex = source.IndexOf(indexBegin) + indexBegin.Length;
            
            var adjustedSource = source[firstIndex..];

            var secondIndex = adjustedSource.IndexOf(indexEnd);

            if (secondIndex == -1)
            {
                return adjustedSource;
            }

            var substringEndIndex = includeIndexEnd ? secondIndex + indexEnd.Length : secondIndex;
            return adjustedSource.Substring(0, substringEndIndex);
        }

        public string GetSubstringBeforeFirstOccurance(string source, string index)
        {
            var firstIndex = source.IndexOf(index);
            var substring = source.Substring(0, firstIndex);

            return substring;
        }

        public string GetSubstringAfterLastOccurance(string source, string index)
        {
            var firstIndex = source.LastIndexOf(index);
            var substring = source[firstIndex..];

            return substring;
        }
    }
}
