using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.Extensions.Logging;
using OOP_Group_Final_Project.Database;
using OOP_Group_Final_Project.Services;

namespace OOP_Group_Final_Project
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services
                .AddBlazorise(options => { options.Immediate = true; })
                .AddBootstrap5Providers()
                .AddFontAwesomeIcons();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            // add services
            builder.Services.AddSingleton<IScheduleService, ScheduleService>();
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<IEmployeeService, EmployeeServiceSQLite>();
            EmployeeDb.InitializeAsync().GetAwaiter().GetResult();
            System.Diagnostics.Debug.WriteLine($"SQLite DB path: {EmployeeDb.DbPath}");
            System.Diagnostics.Debug.WriteLine($"DB exists: {File.Exists(EmployeeDb.DbPath)}");
            return builder.Build();
        }
    }
}
