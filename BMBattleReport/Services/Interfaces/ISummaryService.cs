using BMBattleReport.Models;
using System.Collections.Generic;

namespace BMBattleReport.Services.Interfaces
{
    public interface ISummaryService
    {
        Summary CreateSummary(string source, List<Noble> nobles);
    }
}
