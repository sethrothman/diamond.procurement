using Diamond.Procurement.App.Processing;
using Diamond.Procurement.Data;
using Diamond.Procurement.Data.Contracts;
using Diamond.Procurement.Data.Repositories;
using Diamond.Procurement.Win.Forms;
using Diamond.Procurement.Win.Services;
using Diamond.Procurement.Win.UserControls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;

namespace Diamond.Procurement.Win
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var builder = Host.CreateApplicationBuilder();

            // Config order: base -> environment override -> env vars
            builder.Configuration
                   .AddJsonFile("appsettings.json", optional: true)
                   .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                   .AddEnvironmentVariables();

            // Serilog
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(logDir, "ingest-.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // repositories
            builder.Services.AddSingleton<IDbFactory, SqlDbFactory>();
            builder.Services.AddSingleton<BuyerInventoryRepository>();
            builder.Services.AddSingleton<BuyerForecastRepository>();
            builder.Services.AddSingleton<VendorForecastRepository>();
            builder.Services.AddSingleton<MainframeInventoryRepository>();
            builder.Services.AddSingleton<MasterListRepository>();
            builder.Services.AddSingleton<IImportStatusRepository, ImportStatusRepository>();
            builder.Services.AddSingleton<IOrderVendorShipmentRepository, OrderVendorShipmentRepository>();
            builder.Services.AddTransient<UpcCompRepository>();

            // processors (must be registered since the Func<> factories resolve them)
            builder.Services.AddTransient<LnrInventoryProcessor>();
            builder.Services.AddTransient<LnrForecastProcessor>();
            builder.Services.AddTransient<LibertyForecastProcessor>();
            builder.Services.AddTransient<MainframeInventoryProcessor>();
            builder.Services.AddTransient<CnsInventoryProcessor>();
            builder.Services.AddTransient<DgInventoryProcessor>();
            builder.Services.AddTransient<MeijerInventoryProcessor>();
            builder.Services.AddTransient<UpcCompProcessor>();
            builder.Services.AddTransient<Func<UpcCompProcessor>>(sp => () => sp.GetRequiredService<UpcCompProcessor>());

            // factories for Func<T> used by the handlers
            builder.Services.AddTransient<BuyerInventoryProcessorResolver>(sp => buyerId =>
            {
                return buyerId switch
                {
                    4 => sp.GetRequiredService<DgInventoryProcessor>(), // DG
                    3 => sp.GetRequiredService<MeijerInventoryProcessor>(), // Meijer
                    2 => sp.GetRequiredService<CnsInventoryProcessor>(), // CNS
                    _ => sp.GetRequiredService<LnrInventoryProcessor>(),  // default: LNR
                };
            });
            //builder.Services.AddTransient<Func<LnrInventoryProcessor>>(sp => () => sp.GetRequiredService<LnrInventoryProcessor>());
            builder.Services.AddTransient<Func<LnrForecastProcessor>>(sp => () => sp.GetRequiredService<LnrForecastProcessor>());
            builder.Services.AddTransient<Func<LibertyForecastProcessor>>(sp => () => sp.GetRequiredService<LibertyForecastProcessor>());
            builder.Services.AddTransient<Func<MainframeInventoryProcessor>>(sp => () => sp.GetRequiredService<MainframeInventoryProcessor>());

            // handlers + dispatcher
            builder.Services.AddTransient<IIngestionHandler, BuyerInventoryHandler>();
            builder.Services.AddTransient<IIngestionHandler, BuyerForecastHandler>();
            builder.Services.AddTransient<IIngestionHandler, VendorForecastHandler>();
            builder.Services.AddTransient<IIngestionHandler, MainframeInventoryHandler>();
            builder.Services.AddTransient<IIngestionHandler, UpcCompHandler>();

            builder.Services.AddTransient<IIngestionDispatcher, IngestionDispatcher>();
            builder.Services.AddTransient<IOrderVendorRepository, OrderVendorRepository>();

            // --- UI Orchestrator (Win) ---
            builder.Services.AddTransient<IUiImportOrchestrator, UiImportOrchestrator>();

            builder.Services.AddTransient<IFtpEndpointRepository, FtpEndpointRepository>();

            //forms
            builder.Services.AddSingleton<frmLandingPage>();
            builder.Services.AddTransient<ImportPage>();
            builder.Services.AddTransient<MasterListAnalysisPage>();
            builder.Services.AddTransient<MasterListManagePage>();
            builder.Services.AddTransient<BuyerSoldVendorNotInMaster>();
            builder.Services.AddTransient<OrderVendorShipmentPage>();
            //builder.Services.AddTransient<frmBulkPasteUpcs>();

            var host = builder.Build();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var form = host.Services.GetRequiredService<frmLandingPage>();
            Application.Run(form);
        }
    }
}