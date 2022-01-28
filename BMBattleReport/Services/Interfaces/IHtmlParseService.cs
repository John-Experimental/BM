using BMBattleReport.Models;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace BMBattleReport.Services.Interfaces
{
    public interface IHtmlParseService
    {
        List<Noble> ExtractNoblesInformation(string source);
        List<Army> ExtractArmiesInformation(string source);
        List<Noble> ExtractScoutReportUnitTable(string source);
    }
}
