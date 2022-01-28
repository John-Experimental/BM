namespace BMBattleReport.Models
{
    public class RealmMilitarySummary
    {
        public string RealmName { get; set; }
        public int NumberOfInfantry { get; set; }
        public int TotalInfantryCS { get; set; }
        public int NumberOfArchers { get; set; }
        public int TotalArchersCS { get; set; }
        public int NumberOfCavalry { get; set; }
        public int TotalCavalryCS { get; set;}
        public int NumberOfSpecialForces { get; set; }
        public int TotalSpecialForcesCS { get; set; }
        public int NumberOfMixedInfantry { get; set; }
        public int TotalMixedInfantryCS { get; set; }
    }
}
