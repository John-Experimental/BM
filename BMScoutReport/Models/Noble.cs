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

        //Dictionary is build up like: target-{roundnr}, hits
        public Dictionary<string, int> HitsScored { get; set; } 
        public Dictionary<int, int> HitsTaken { get; set; }
        public Dictionary<int, int> CasualtiesTaken { get; set; }
        public Dictionary<int, int> HitsPerCasualty { get; set; }
        public Dictionary<int, int> CasualtiesInflicted { get; set; }

        public Noble()
        {
            HitsScored = new Dictionary<string, int>();
            HitsTaken = new Dictionary<int, int>();
            CasualtiesTaken = new Dictionary<int, int>();
            HitsPerCasualty = new Dictionary<int, int>();
            CasualtiesInflicted = new Dictionary<int, int>();
        }
    }
}
