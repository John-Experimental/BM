using BMBattleReport.Constants;
using BMBattleReport.Models;
using BMBattleReport.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace BMBattleReport.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly IHelperService _helperService;
        private readonly IHtmlParseService _htmlParseService;

        private readonly Dictionary<string, string> _weatherOptions = new()
        {
            { "There is almost no wind today, archers will be deadly", "No wind" },
            { "A calm wind blows, to the joy of the archers", "Calm wind" },
            { "It is quite windy and the archers will have to aim very carefully", "Quite windy" },
            { "Strong winds and gusts are making ranged combat a game of luck", "Strong wind" },
            { "The battle takes place in the middle of a storm, archers will be almost worthless", "Storm" }
        };

        private readonly List<string> _outcomeOptions = new() { "Attacker Victory", "Defender Victory", "Draw" };

        public SummaryService(IHelperService helperService, IHtmlParseService htmlParseService)
        {
            _helperService = helperService;
            _htmlParseService = htmlParseService;
        }

        public Summary CreateSummary(string source, List<Noble> nobles)
        {
            var region = _helperService.GetSubstringMiddle(source, "Battle in ", "<br");

            var regionOwner = _helperService.GetSubstringMiddle(source, "The region owner ", " and their allies defend");
            if (regionOwner.Length > 30) { regionOwner = "Unknown"; };

            var weather = _weatherOptions.First(option => source.Contains(option.Key)).Value;
            var outcome = _outcomeOptions.First(option => source.Contains(option));

            var attackingNobles = nobles.Where(noble => noble.Role == "A").ToList();
            var defendingNobles = nobles.Where(noble => noble.Role == "D").ToList();

            var attackingRealms = attackingNobles.GroupBy(noble => new { noble.Realm }).Select(x => x.Key).Select(t => t.Realm).ToList();
            var defendingRealms = defendingNobles.GroupBy(noble => new { noble.Realm }).Select(x => x.Key).Select(t => t.Realm).ToList();

            var armies = _htmlParseService.ExtractArmiesInformation(source);
            var attackingArmies = GetAttackingArmies(armies, attackingNobles);
            var defendingArmies = armies.Except(attackingArmies).ToList();

            var strengthAttackers = _helperService.GetSubstringMiddle(source, " attackers (", ")<br");
            var strengthDefenders = _helperService.GetSubstringMiddle(source, " defenders (", ")<br");

            var casualtiesAttackers = attackingNobles.Sum(x => x.CasualtiesTakenPerRound.Values.Sum());
            var casualtiesDefenders = defendingNobles.Sum(x => x.CasualtiesTakenPerRound.Values.Sum());

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
                DefendersCommanders = defendingArmies.Select(a => a.Commander).ToList(),
                DefendersFormations = defendingArmies.Select(a => a.Formation).ToList(),
                Attackers = attackingRealms,
                AttackersStrength = strengthAttackers,
                AttackersCasualties = casualtiesAttackers,
                AttackersCommanders = attackingArmies.Select(a => a.Commander).ToList(),
                AttackersFormations = attackingArmies.Select(a => a.Formation).ToList(),
                NumberOfRounds = totalRounds
            };
        }

        private static List<Army> GetAttackingArmies(List<Army> allArmies, List<Noble> attackingNobles)
        {
            return allArmies.Where(army => attackingNobles.Any(noble => noble.NobleName.Split(CommonCharacters.WhiteSpace).Length == 1 ?
                    noble.NobleName == army.Commander.Split(CommonCharacters.WhiteSpace)[0] :
                    noble.NobleName == string.Join(CommonCharacters.WhiteSpace, army.Commander.Split(CommonCharacters.WhiteSpace).Take(noble.NobleName.Split(CommonCharacters.WhiteSpace).Length))))
                .ToList();
        }
    }
}
