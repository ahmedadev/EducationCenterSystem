using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EducationCenterSystem.Presentation.WinForms.Services;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Views;

namespace EducationCenterSystem.Presentation.WinForms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Core Services
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<ITokenProvider, TokenProvider>();
                services.AddTransient<AuthenticatedHttpClientHandler>();
                services.AddTransient<IAuthApiService, AuthApiService>();
                services.AddTransient<ITeacherApiService, TeacherApiService>();
                services.AddTransient<IStudentApiService, StudentApiService>();

                // HTTP Client configuration
                services.AddHttpClient(string.Empty, client =>
                {
                    var baseUrl = context.Configuration["ApiSettings:BaseUrl"] ?? "http://127.0.0.1:5145/";
                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromSeconds(5);
                })
                .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

                // UserControls (Views)
                services.AddTransient<StudentsView>();
                services.AddTransient<TeachersView>();
                services.AddTransient<CoursesAndGroupsView>();
                services.AddTransient<AttendanceView>();
                services.AddTransient<AdminDashboardView>();
                services.AddTransient<LoginView>();
                services.AddTransient<RegisterView>();

                // Main Form
                services.AddSingleton<MainForm>();
            })
            .Build();

        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }
}