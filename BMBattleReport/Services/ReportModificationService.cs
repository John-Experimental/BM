using BMBattleReport.Constants;
using BMBattleReport.Models;
using BMBattleReport.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
                // First mentions of the nobles is in the first table, which we want to expand here. So IndexOf works perfectly
                index = result.IndexOf($">{noble.No}</td>");
                indexEndRow = result.IndexOf("</tr>", index);

                hitsScored = noble.HitsScoredPerRoundPerTarget.Sum(round => round.Value.Sum(hits => hits.Value));
                casualtiesInflicted = noble.CasualtiesInflictedPerRound.Sum(round => round.Value);
                casualtiesTaken = noble.CasualtiesTakenPerRound.Sum(round => round.Value);

                // Working with the placeholders because otherwise if the number of casualties/hits scored by a previous noble might conflict with the index of the noble
                additionalStats = $"<td style=\"text-align: right; \">{hitsScored}placeholder</td><td style=\"text-align: center; \">{casualtiesInflicted}placeholder</td><td style=\"text-align: center; \">{casualtiesTaken}placeholder</td>";

                result = result.Insert(indexEndRow, additionalStats);
            }

            return result.Replace("placeholder", string.Empty);
        }

        public string AddSummaryToSource(string source, Summary summary)
        {
            var infoBox = $"{{{{Infobox Military Conflict |conflict= Battle of {summary.Region} |partof= {summary.RegionOwner} |image= |caption= " +
                $"|date= {DateTime.Today.ToShortDateString()} |place= [[{summary.Region}]] |weather= {summary.Weather} |territory=none |result= {summary.Outcome} |combatant1= {string.Join("; ", summary.Attackers)}" +
                $"|combatant2= {string.Join("; ", summary.Defenders)} |commander1= {string.Join("; ", summary.AttackersCommanders)} |commander2= {string.Join("; ", summary.DefendersCommanders)} |strength1= {summary.AttackersStrength}" +
                $"|strength2= {summary.DefendersStrength} |formation1= {string.Join("; ", summary.AttackersFormations)} |formation2= {string.Join("; ", summary.DefendersFormations)} |rounds= {summary.NumberOfRounds}" +
                $"|casualties1= {summary.AttackersCasualties} |casualties2= {summary.DefendersCasualties}}}}}__NOTOC__";

            return $"{infoBox} {source}";
        }

        public string MakeNobleTableSortable(string source)
        {
            var firstTableIndex = source.IndexOf(CommonCharacters.TableOpening);
            return source.Insert(firstTableIndex + CommonCharacters.TableOpening.Length, "class=sortable ");
        }
    }
}
