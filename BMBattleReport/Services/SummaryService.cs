using BMBattleReport.Models;
using BMBattleReport.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BMBattleReport.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly IHelperService _helperService;
        private readonly Dictionary<string, string> _weatherOptions = new() { 
            { "There is almost no wind today, archers will be deadly", "No wind" }, 
            { "A calm wind blows, to the joy of the archers", "Calm wind" }, 
            { "It is quite windy and the archers will have to aim very carefully", "Quite windy" }, 
            { "Strong winds and gusts are making ranged combat a game of luck", "Strong wind" }, 
            { "The battle takes place in the middle of a storm, archers will be almost worthless", "Storm" } };

        private readonly List<string> _outcomeOptions = new() { "Attacker Victory", "Defender Victory", "Draw" };

        public SummaryService(IHelperService helperService)
        {
            _helperService = helperService;
        }

        public Summary CreateSummary(string source, List<Noble> nobles)
        {
            var region = _helperService.GetSubstringMiddle(source, "Battle in ", "<br");

            var regionOwner = _helperService.GetSubstringMiddle(source, "The region owner ", " and their allies defend");
            if (regionOwner.Length > 30) { regionOwner = "Unknown"; };

            var weather = _weatherOptions.First(option => source.Contains(option.Key)).Value;
            var outcome = _outcomeOptions.First(option => source.Contains(option));

            var attackingRealms = nobles.Where(noble => noble.Role == "A").GroupBy(noble => new { noble.Realm }).Select(x => x.Key).Select(t => t.Realm).ToList();
            var defendingRealms = nobles.Where(noble => noble.Role == "D").GroupBy(noble => new { noble.Realm }).Select(x => x.Key).Select(t => t.Realm).ToList();

            var strengthAttackers = _helperService.GetSubstringMiddle(source, " attackers (", ")<br");
            var strengthDefenders = _helperService.GetSubstringMiddle(source, " defenders (", ")<br");

            var casualtiesAttackers = nobles.Where(noble => noble.Role == "A").Sum(x => x.CasualtiesTakenPerRound.Values.Sum());
            var casualtiesDefenders = nobles.Where(noble => noble.Role == "D").Sum(x => x.CasualtiesTakenPerRound.Values.Sum());

            var totalRounds = nobles.Max(noble => noble.HitsTakenPerRound.LastOrDefault().Key);

            return new Summary
            {
                Region = region,
                RegionOwner = regionOwner,
                Weather = weather,
                Outcome = outcome,
                Defenders = defendingRealms,
                DefendersStrength = strengthDefenders,
                DefendersCasualties = casualtiesDefenders,
                Attackers = attackingRealms,
                AttackersStrength = strengthAttackers,
                AttackersCasualties = casualtiesAttackers,
                NumberOfRounds = totalRounds
            };
        }
    }
}
