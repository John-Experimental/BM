using BMBattleReport.Services.Interfaces;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BMBattleReport.Pages
{
    public partial class Index
    {
        [Inject]
        public IHtmlCleanUpService _htmlCleanUpService { get; set; }
        [Inject]
        public IHtmlParseService _htmlParseService { get; set; }

        [Required]
        public string ReportInput = string.Empty;
        public string ReportOutput;

        private void TransformReport()
        {
            var cleanedUpReport = _htmlCleanUpService.CleanUpHTML(ReportInput);
            var nobles = _htmlParseService.ExtractNoblesInformation(cleanedUpReport);

            ReportOutput = cleanedUpReport;
            ReportInput = string.Empty;
        }
    }
}
