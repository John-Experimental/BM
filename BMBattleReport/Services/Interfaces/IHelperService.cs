namespace BMBattleReport.Services.Interfaces
{
    public interface IHelperService
    {
        string GetSubstringMiddle(string source, string indexBegin, string indexEnd, bool includeIndexEnd = false);
        string GetSubstringBeforeFirstOccurance(string source, string index);
        string GetSubstringAfterLastOccurance(string source, string index);
    }
}
