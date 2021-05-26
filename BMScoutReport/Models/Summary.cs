using System.Collections.Generic;

namespace BMBattleReport.Models
{
    public class Summary
    {
        public string Region { get; init; }
        public string RegionOwner { get; init; }
        public string Weather { get; init; }
        public string Outcome { get; init; }
        
        public List<string> Defenders { get; init; }
        public string DefendersStrength { get; init; }
        public int DefendersCasualties { get; init; }
        public List<string> DefendersCommanders { get; init; }
        public List<string> DefendersFormations { get; init; }

        public List<string> Attackers { get; init; }
        public string AttackersStrength { get; init; }
        public int AttackersCasualties { get; init; }
        public List<string> AttackersCommanders { get; init; }
        public List<string> AttackersFormations { get; init; }

    }
}
