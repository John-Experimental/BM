using BMBattleReport.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BMBattleReport.Pages
{
    public partial class Index
    {
        [Inject]
        public IHtmlCleanUpService _htmlCleanUpService { get; set; }

        [Required]
        public string ReportInput = string.Empty;
        public string ReportOutput;

        private void TransformReport()
        {
            ReportOutput = _htmlCleanUpService.CleanUpHTML(ReportInput);
            ReportInput = string.Empty;

            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
    }
}
