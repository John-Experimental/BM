using System.Collections.Generic;

namespace BMBattleReport.Models
{
    public class Noble
    {
        public string No { get; set; }
        public string Role { get; set; }
        public string UnitName { get; set; }
        public string NobleName { get; set; }
        public string Realm { get; set; }
        public string UnitSize { get; set; }
        public string UnitType { get; set; }
        public string UnitFormation { get; set; }
        public string CS { get; set; }

        public Dictionary<int, Dictionary<string, int>> HitsScoredPerRoundPerTarget { get; set; } 
        public Dictionary<int, int> HitsTakenPerRound { get; set; }
        public Dictionary<int, int> CasualtiesTakenPerRound { get; set; }
        public Dictionary<int, decimal> DivisionHitsCasualtyPerRound { get; set; }
        public Dictionary<int, decimal> CasualtiesInflictedPerRound { get; set; }

        public Noble()
        {
            HitsScoredPerRoundPerTarget = new Dictionary<int, Dictionary<string, int>>();
            HitsTakenPerRound = new Dictionary<int, int>();
            CasualtiesTakenPerRound = new Dictionary<int, int>();
            DivisionHitsCasualtyPerRound = new Dictionary<int, decimal>();
            CasualtiesInflictedPerRound = new Dictionary<int, decimal>();
        }
    }
}
