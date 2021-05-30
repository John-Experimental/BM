using BMBattleReport.Services;
using BMBattleReport.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BMBattleReport
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IHtmlCleanUpService, HtmlCleanUpService>();
            builder.Services.AddScoped<IHtmlParseService, HtmlParseService>();
            builder.Services.AddScoped<IHelperService, HelperService>();
            builder.Services.AddScoped<IReportModificationService, ReportModificationService>();
            builder.Services.AddScoped<ISummaryService, SummaryService>();

            await builder.Build().RunAsync();
        }
    }
}
