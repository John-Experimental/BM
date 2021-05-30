﻿using BMBattleReport.Constants;
using BMBattleReport.Models;
using BMBattleReport.Services.Interfaces;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace BMBattleReport.Services
{
    public class HtmlParseService : IHtmlParseService
    {
        private readonly IHelperService _helperService;
        private readonly string[] _hitsScoredIndicators = new string[] { "fire on", "score" };

        public HtmlParseService(IHelperService helperService)
        {
            _helperService = helperService;
        }

        public List<Noble> ExtractNoblesInformation(string source)
        {
            var html = new HtmlDocument();
            html.LoadHtml(source);

            var nobles = ExtractNoblesBasicInformation(html);
            nobles = ExtractCombatResultsForNobles(nobles, source);

            return nobles;
        }

        private List<Noble> ExtractCombatResultsForNobles(List<Noble> nobles, string source)
        {
            var roundNumber = 1;
            string currentTurn;
            string nextTurn;
            string textCurrentRound;
            do
            {
                currentTurn = $"Turn No. {roundNumber}";
                nextTurn = $"Turn No. {roundNumber + 1}";
                textCurrentRound = _helperService.GetSubstringMiddle(source, currentTurn, nextTurn);

                foreach (var noble in nobles)
                {
                    noble.HitsScoredPerRoundPerTarget[roundNumber] = new Dictionary<string, int>();

                    AssignHitsInflicted(textCurrentRound, noble, roundNumber);
                    AssignStatsSuffered(textCurrentRound, noble, roundNumber);
                    //nobles = GetCasualtiesInflicted(nobles, roundNumber);
                }

                ++roundNumber;
            }
            while (source.Contains(nextTurn));

            return nobles;
        }

        private List<Noble> ExtractNoblesBasicInformation(HtmlDocument htmlSource)
        {
            var combatantsOverviewTable = htmlSource.DocumentNode.SelectNodes("/table").First();
            var tableRows = combatantsOverviewTable.ChildNodes;

            var nobles = new List<Noble>();

            foreach (var row in tableRows)
            {
                if (row.OuterHtml.Contains("<td>"))
                {
                    var td = row.ChildNodes;

                    string nobleName;
                    if (td[4].InnerHtml.Contains("militia/guard unit"))
                    {
                        nobleName = "militia / guard unit";
                    }
                    else
                    {
                        nobleName = td[4].InnerHtml.Replace(CommonCharacters.Space, string.Empty);
                    }

                    var newNoble = new Noble
                    {
                        No = td[0].InnerHtml.Replace(CommonCharacters.Space, string.Empty),
                        Role = td[1].InnerHtml.Replace(CommonCharacters.Space, string.Empty),
                        UnitName = td[3].InnerHtml.Replace(CommonCharacters.Space, string.Empty),
                        NobleName = nobleName,
                        Realm = td[5].InnerHtml.Replace(CommonCharacters.Space, string.Empty),
                        UnitSize = td[6].InnerHtml.Split(CommonCharacters.Space)[0],
                        UnitType = td[6].InnerHtml.Split(CommonCharacters.Space)[1],
                        UnitFormation = td[7].InnerHtml.Replace(CommonCharacters.Space, string.Empty),
                        CS = td[8].InnerHtml.Replace(CommonCharacters.Space, string.Empty),
                    };

                    nobles.Add(newNoble);
                }
            }
            return nobles;
        }

        private void AssignHitsInflicted(string source, Noble noble, int round)
        {
            string subSource;

            foreach (var indicator in _hitsScoredIndicators)
            {
                var hitsInflictedIndicator = $"{noble.UnitName} ({noble.No}) {indicator}";
                subSource = source;

                AssignAllOccurancesToNoble(subSource, hitsInflictedIndicator, noble, round);
            }
        }

        private void AssignAllOccurancesToNoble(string subSource, string hitsInflictedIndicator, Noble noble, int round)
        {
            int hits;
            string target;

            var indexOfHitsInflicted = subSource.IndexOf(hitsInflictedIndicator);

            while (indexOfHitsInflicted >= 0)
            {
                indexOfHitsInflicted = subSource.IndexOf(hitsInflictedIndicator);

                if (indexOfHitsInflicted == -1) { break; }

                subSource = subSource[indexOfHitsInflicted..];

                hits = GetNumberBeforeIndicator(subSource, CommonCharacters.HitsInflictedIndicator);
                target = _helperService.GetSubstringMiddle(subSource, CommonCharacters.TargetIndicator, CommonCharacters.HitsEnding, true);

                if (noble.HitsScoredPerRoundPerTarget[round].ContainsKey(target))
                {
                    noble.HitsScoredPerRoundPerTarget[round][target] += hits;
                }
                else
                {
                    noble.HitsScoredPerRoundPerTarget[round].Add(target, hits);
                }

                //This ensures we keep looking in the rest of the text whether or not it occurs again
                subSource = subSource[hitsInflictedIndicator.Length..];
            }
        }

        private void AssignStatsSuffered(string subSource, Noble noble, int round)
        {
            int hits;
            int casualties;

            var hitsSufferedIndicator = $"{noble.UnitName} ({noble.No}){CommonCharacters.HitsSufferedIndicator}";
            var indexOfHitsSuffered = subSource.IndexOf(hitsSufferedIndicator);

            if (indexOfHitsSuffered >= 0)
            {
                subSource = subSource[indexOfHitsSuffered..];
                hits = GetNumberBeforeIndicator(subSource, CommonCharacters.HitsInflictedIndicator);
                casualties = GetNumberBeforeIndicator(subSource, CommonCharacters.CasualtiesIndicator);

                noble.HitsTakenPerRound[round] = hits;
                noble.CasualtiesTakenPerRound[round] = casualties;
                noble.DivisionHitsCasualtyPerRound[round] = hits / casualties;
            }
        }

        private int GetNumberBeforeIndicator(string source, string indicator)
        {
            var subResult = _helperService.GetSubstringBeforeFirstOccurance(source, indicator);
            subResult = _helperService.GetSubstringAfterLastOccurance(subResult, CommonCharacters.WhiteSpace);

            return int.Parse(subResult);
        }
    }
}
