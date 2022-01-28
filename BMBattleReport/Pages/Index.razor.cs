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
        [Inject]
        public ISummaryService _summaryService { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; }

        [Required]
        public string ReportInput = string.Empty;
        public string ReportOutput;

        protected override void OnInitialized()
        {
            ReportOutput = $"baseuri: {NavManager.BaseUri}";
        }

        private void TestNavigation()
        {
            NavManager.NavigateTo("/ScoutReport/", forceLoad: true);
        }

        private void TransformReport()
        {
            var cleanedUpReport = _htmlCleanUpService.CleanUpHTML(ReportInput);
            var nobles = _htmlParseService.ExtractNoblesInformation(cleanedUpReport);
            var expandedReport = _reportModificationService.InsertNobleStatsIntoSource(cleanedUpReport, nobles);

            var summary = _summaryService.CreateSummary(expandedReport, nobles);
            var reportWithInfoBox = _reportModificationService.AddSummaryToSource(expandedReport, summary);

            ReportOutput = reportWithInfoBox;
            ReportInput = string.Empty;
        }
    }
}
