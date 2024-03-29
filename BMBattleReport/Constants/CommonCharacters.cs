﻿using System.Collections.Generic;

namespace BMBattleReport.Constants
{
    public class CommonCharacters
    {
        public const string Space = "&nbsp;";
        public const string WhiteSpace = " ";
        public const string Period = ".";

        public static readonly List<string> HitsEnding = new() { "),", ")."};
        public const string OpeningParenthesis = "(";
        public const string TargetIndicator = " on ";
        public const string HitsInflictedIndicator = " hits";
        public const string HitsSufferedIndicator = " take ";
        public const string CasualtiesIndicator = " casualties";

        public const string CommanderSeparator = ",";
        public const string ClosingTag = ">";

        public const string TableOpening = "<table ";
    }
}
