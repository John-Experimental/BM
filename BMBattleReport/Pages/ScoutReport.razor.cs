using BMBattleReport.Models;
using BMBattleReport.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BMBattleReport.Pages
{
    public partial class ScoutReport
    {
        [Inject]
        public IHtmlParseService _htmlParseService { get; set; }

        [Required]
        public string ReportInput = string.Empty;

        public List<RealmMilitarySummary> Summaries = new();

        private void TransformReport()
        {
            var nobles = _htmlParseService.ExtractScoutReportUnitTable(ReportInput);
            var realms = nobles.Select(n => n.Realm).Distinct();

            foreach (var realm in realms)
            {
                var noblesInRealm = nobles.Where(n => n.Realm == realm).ToList();
                var mobileForces = noblesInRealm.Where(n => n.NobleName != "militia / guard unit").ToList();
                var militiaForce = noblesInRealm.Except(mobileForces).ToList();

                AddRealmSummary(realm, mobileForces);

                if (militiaForce.Any())
                {
                    AddRealmSummary($"{realm} militia force", militiaForce);
                }
            }
        }

        private void AddRealmSummary(string realmName, List<Noble> nobles)
        {
            Summaries.Add(new RealmMilitarySummary()
            {
                RealmName = realmName,
                NumberOfInfantry = nobles.Where(f => f.UnitType == "Infantry").Sum(m => int.Parse(m.UnitSize)),
                TotalInfantryCS = nobles.Where(f => f.UnitType == "Infantry").Sum(m => int.Parse(m.CS)),
                NumberOfArchers = nobles.Where(f => f.UnitType == "Archers").Sum(m => int.Parse(m.UnitSize)),
                TotalArchersCS = nobles.Where(f => f.UnitType == "Archers").Sum(m => int.Parse(m.CS)),
                NumberOfCavalry = nobles.Where(f => f.UnitType == "Cavalry").Sum(m => int.Parse(m.UnitSize)),
                TotalCavalryCS = nobles.Where(f => f.UnitType == "Cavalry").Sum(m => int.Parse(m.CS)),
                NumberOfMixedInfantry = nobles.Where(f => f.UnitType == "Mixed Infantry").Sum(m => int.Parse(m.UnitSize)),
                TotalMixedInfantryCS = nobles.Where(f => f.UnitType == "Mixed Infantry").Sum(m => int.Parse(m.CS)),
                NumberOfSpecialForces = nobles.Where(f => f.UnitType == "Special Forces").Sum(m => int.Parse(m.UnitSize)),
                TotalSpecialForcesCS = nobles.Where(f => f.UnitType == "Special Forces").Sum(m => int.Parse(m.CS))
            });
        }
    }
}
