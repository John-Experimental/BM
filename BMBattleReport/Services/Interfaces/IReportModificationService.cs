using BMBattleReport.Models;
using System.Collections.Generic;

namespace BMBattleReport.Services.Interfaces
{
    public interface IReportModificationService
    {
        string InsertNobleStatsIntoSource(string source, List<Noble> nobles);
        string AddSummaryToSource(string source, Summary summary);
        string MakeNobleTableSortable(string source);
    }
}
