using BMBattleReport.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BMBattleReport.Pages
{
    public partial class Index
    {
        [Inject]
        public IHtmlCleanUpService _htmlCleanUpService { get; set; }
        [Inject]
        public IHtmlParseService _htmlParseService { get; set; }
        [Inject]
        public IReportModificationService _reportModificationService { get; set; }

        [Required]
        public string ReportInput = string.Empty;
        public string ReportOutput;

        private void TransformReport()
        {
            var cleanedUpReport = _htmlCleanUpService.CleanUpHTML(ReportInput);
            var nobles = _htmlParseService.ExtractNoblesInformation(cleanedUpReport);
            var expandedReport = _reportModificationService.InsertNobleStatsIntoSource(cleanedUpReport, nobles);

            ReportOutput = expandedReport;
            ReportInput = string.Empty;
        }
    }
}
