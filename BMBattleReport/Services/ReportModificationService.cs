using BMBattleReport.Models;
using BMBattleReport.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BMBattleReport.Services
{
    public class ReportModificationService : IReportModificationService
    {
        public string InsertNobleStatsIntoSource(string source, List<Noble> nobles)
        {
            var indexEndRow = source.IndexOf("</tr>");
            var additionalStats = "<th>Hits Scored</th><th>Soldiers killed</th><th>Soldiers lost</th>";
            var result = source.Insert(indexEndRow, additionalStats);

            int index; 
            int hitsScored;
            decimal casualtiesInflicted;
            int casualtiesTaken;

            foreach (var noble in nobles)
            {
                //First mentions of the nobles is in the first table, which we want to expand here. So IndexOf works perfectly
                index = result.IndexOf($">{noble.No}</td>");
                indexEndRow = result.IndexOf("</tr>", index);

                hitsScored = noble.HitsScoredPerRoundPerTarget.Sum(round => round.Value.Sum(hits => hits.Value));
                casualtiesInflicted = noble.CasualtiesInflictedPerRound.Sum(round => round.Value);
                casualtiesTaken = noble.CasualtiesTakenPerRound.Sum(round => round.Value);

                additionalStats = $"<td style=\"text-align: right; \">{hitsScored} hits</td><td style=\"text-align: center; \">{casualtiesInflicted} kills</td><td style=\"text-align: center; \">{casualtiesTaken} losses</td>";

                result = result.Insert(indexEndRow, additionalStats);
            }

            return result;
        }
    }
}
