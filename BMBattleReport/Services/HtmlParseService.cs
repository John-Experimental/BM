﻿using BMBattleReport.Constants;
using BMBattleReport.Models;
using BMBattleReport.Services.Interfaces;
using HtmlAgilityPack;
using System;
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
            nobles = CalculateAdditionalStatsForNobles(nobles);

            return nobles;
        }

        public List<Army> ExtractArmiesInformation(string source)
        {
            var armies = new List<Army>();

            var relevantSection = _helperService.GetSubstringMiddle(source, "Total combat strengths:", "Turn No. 1");
            var armySections = relevantSection.Split("<img src=").ToArray();

            string commander;
            string formation;
            for (int i = 1; i < armySections.Length; i++)
            {
                commander = _helperService.GetSubstringMiddle(armySections[i], CommonCharacters.ClosingTag, CommonCharacters.CommanderSeparator).Trim();
                formation = _helperService.GetSubstringMiddle(armySections[i], "They deploy in", CommonCharacters.Period).Trim();

                armies.Add(new Army 
                { 
                    Commander = commander, 
                    Formation = formation 
                });
            }

            return armies;
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
                }

                ++roundNumber;
            }
            while (source.Contains(nextTurn));

            return nobles;
        }

        private static List<Noble> ExtractNoblesBasicInformation(HtmlDocument htmlSource)
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

        public List<Noble> ExtractScoutReportUnitTable(string source)
        {
            var html = new HtmlDocument();
            html.LoadHtml(source);

            var unitsOverviewTable = html.DocumentNode.SelectNodes("//table[contains(@class, 'tabtable')]")[1];
            var table = unitsOverviewTable.ChildNodes;

            var nobles = new List<Noble>();

            foreach (var section in table)
            {
                var rows = section.ChildNodes;
                
                foreach (var row in rows)
                {
                    if (row.OuterHtml.Contains("<td>"))
                    {
                        var td = row.ChildNodes;

                        string nobleName;
                        Console.WriteLine(td[4].InnerHtml);
                        if (td[2].InnerHtml.Contains("(militia/guards)"))
                        {
                            nobleName = "militia / guard unit";
                        }
                        else
                        {
                            nobleName = td[2].InnerHtml.Replace(CommonCharacters.Space, string.Empty);
                        }

                        var newNoble = new Noble
                        {
                            UnitName = td[1].InnerHtml.Replace(CommonCharacters.Space, string.Empty),
                            NobleName = nobleName,
                            Realm = td[3].InnerHtml.Replace(CommonCharacters.Space, string.Empty),
                            UnitSize = td[4].InnerHtml.Split(CommonCharacters.WhiteSpace, 2)[0],
                            UnitType = td[4].InnerHtml.Split(CommonCharacters.WhiteSpace, 2)[1],
                            CS = td[6].InnerHtml,
                        };

                        nobles.Add(newNoble);
                    }
                }
            }

            return nobles;
        }

        private void AssignHitsInflicted(string source, Noble noble, int round)
        {
            string subSource;

            foreach (var indicator in _hitsScoredIndicators)
            {
                var hitsInflictedIndicator = string.IsNullOrEmpty(noble.UnitName) ? $"({noble.No}) {indicator}" : $"{noble.UnitName} ({noble.No}) {indicator}";
                subSource = source;

                AssignAllOccurancesToNoble(subSource, hitsInflictedIndicator, noble, round);
            }
        }

        private void AssignAllOccurancesToNoble(string subSource, string hitsInflictedIndicator, Noble noble, int round)
        {
            int hits;
            string targetNobleNumber;

            var indexOfHitsInflicted = subSource.IndexOf(hitsInflictedIndicator);

            while (indexOfHitsInflicted >= 0)
            {
                indexOfHitsInflicted = subSource.IndexOf(hitsInflictedIndicator);

                if (indexOfHitsInflicted == -1) { break; }

                subSource = subSource[indexOfHitsInflicted..];

                hits = GetNumberBeforeIndicator(subSource, CommonCharacters.HitsInflictedIndicator);

                if (hits > 0)
                {
                    targetNobleNumber = _helperService.GetSubstringMiddle(subSource, CommonCharacters.TargetIndicator, CommonCharacters.HitsEnding).Split(CommonCharacters.OpeningParenthesis).Last();

                    if (targetNobleNumber.Contains("overkill"))
                    {
                        targetNobleNumber = _helperService.GetSubstringMiddle(subSource[..(subSource.Length - 3)], CommonCharacters.TargetIndicator, ")").Split(CommonCharacters.OpeningParenthesis).Last();
                    }

                    if (noble.HitsScoredPerRoundPerTarget[round].ContainsKey(targetNobleNumber))
                    {
                        noble.HitsScoredPerRoundPerTarget[round][targetNobleNumber] += hits;
                    }
                    else
                    {
                        noble.HitsScoredPerRoundPerTarget[round].Add(targetNobleNumber, hits);
                    }
                }

                //This ensures we keep looking in the rest of the text whether or not it occurs again
                subSource = subSource[hitsInflictedIndicator.Length..];
            }
        }

        private void AssignStatsSuffered(string subSource, Noble noble, int round)
        {
            int hits;
            int casualties;

            var hitsSufferedIndicator = string.IsNullOrEmpty(noble.UnitName) ? 
                $"({noble.No}){CommonCharacters.HitsSufferedIndicator}" :  $"{noble.UnitName} ({noble.No}){CommonCharacters.HitsSufferedIndicator}";

            var indexOfHitsSuffered = subSource.IndexOf(hitsSufferedIndicator);

            if (indexOfHitsSuffered >= 0)
            {
                subSource = subSource[indexOfHitsSuffered..];
                hits = GetNumberBeforeIndicator(subSource, CommonCharacters.HitsInflictedIndicator);
                casualties = GetNumberBeforeIndicator(subSource, CommonCharacters.CasualtiesIndicator);

                noble.HitsTakenPerRound[round] = hits;
                noble.CasualtiesTakenPerRound[round] = casualties;
                noble.DivisionHitsCasualtyPerRound[round] = casualties > 0 ? decimal.Divide(hits,casualties) : 0;
            }
        }

        private int GetNumberBeforeIndicator(string source, string indicator)
        {
            var subResult = _helperService.GetSubstringBeforeFirstOccurance(source, indicator);
            subResult = _helperService.GetSubstringAfterLastOccurance(subResult, CommonCharacters.WhiteSpace);

            if (subResult.Contains("no", System.StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            return int.Parse(subResult);
        }

        private static List<Noble> CalculateAdditionalStatsForNobles(List<Noble> nobles)
        {
            foreach (var noble in nobles)
            {
                foreach (var round in noble.HitsScoredPerRoundPerTarget)
                {
                    noble.CasualtiesInflictedPerRound[round.Key] = 0;

                    foreach (var hitsScored in round.Value)
                    {
                        var targetNoble = nobles.First(n => n.No == hitsScored.Key);

                        if (targetNoble.DivisionHitsCasualtyPerRound[round.Key] > 0)
                        {
                            var casualtiesInflicted = Math.Round(decimal.Divide(hitsScored.Value, targetNoble.DivisionHitsCasualtyPerRound[round.Key]), 0);

                            noble.CasualtiesInflictedPerRound[round.Key] += casualtiesInflicted;
                        }
                    }
                }
            }

            return nobles;
        }
    }
}
