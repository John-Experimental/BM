using BMBattleReport.Models;
using System.Collections.Generic;

namespace BMBattleReport.Services.Interfaces
{
    public interface IHtmlParseService
    {
        List<Noble> ExtractNoblesInformation(string source);
    }
}
