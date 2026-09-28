using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EducationCenterSystem.Presentation.Services;
using EducationCenterSystem.Presentation.Services.Abstractions;
using EducationCenterSystem.Presentation.ViewModels;
using EducationCenterSystem.Presentation.Views;

namespace EducationCenterSystem.Presentation;

public partial class App : System.Windows.Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register Services
                services.AddSingleton<IDialogService, DialogService>();
                services.AddSingleton<ITokenProvider, TokenProvider>();
                services.AddTransient<AuthenticatedHttpClientHandler>();

                // Register Http Client with centralized BaseAddress from appsettings.json
                services.AddHttpClient(string.Empty, client =>
                {
                    var baseUrl = context.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5145/";
                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromSeconds(5); // Fast timeout if API is down
                })
                .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

                // Register ViewModels
                services.AddTransient<MainViewModel>();
                services.AddTransient<RegisterStudentViewModel>();
                services.AddTransient<StudentsListViewModel>();
                services.AddTransient<RegisterTeacherViewModel>();
                services.AddTransient<TeachersListViewModel>();
                services.AddTransient<AdminDashboardViewModel>();
                services.AddTransient<CoursesAndGroupsViewModel>();
                services.AddTransient<AttendanceViewModel>();

                // Register Views
                services.AddTransient<MainWindow>();
            })
            .Build();

        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            System.IO.File.WriteAllText("crash.log", e.Exception.ToString());
            e.SetObserved();
        };
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        try
        {
            var httpClientFactory = _host.Services.GetRequiredService<IHttpClientFactory>();
            var tokenProvider = _host.Services.GetRequiredService<ITokenProvider>();
            var client = httpClientFactory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("api/auth/login", new
            {
                email = "admin@educationcenter.com",
                password = "Admin123456!"
            });

            if (loginResponse.IsSuccessStatusCode)
            {
                var authResult = await loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                string token = authResult.TryGetProperty("token", out var tProp) ? (tProp.GetString() ?? string.Empty) : string.Empty;
                string firstName = authResult.TryGetProperty("firstName", out var fnProp) ? (fnProp.GetString() ?? string.Empty) : string.Empty;
                string lastName = authResult.TryGetProperty("lastName", out var lnProp) ? (lnProp.GetString() ?? string.Empty) : string.Empty;
                string email = authResult.TryGetProperty("email", out var emProp) ? (emProp.GetString() ?? string.Empty) : string.Empty;

                var roles = new List<string>();
                if (authResult.TryGetProperty("roles", out var rProp) && rProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var r in rProp.EnumerateArray())
                    {
                        var val = r.GetString();
                        if (!string.IsNullOrWhiteSpace(val)) roles.Add(val);
                    }
                }

                var permissions = new List<string>();
                if (authResult.TryGetProperty("permissions", out var pProp) && pProp.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var p in pProp.EnumerateArray())
                    {
                        var val = p.GetString();
                        if (!string.IsNullOrWhiteSpace(val)) permissions.Add(val);
                    }
                }

                tokenProvider.SetAuthentication(token, $"{firstName} {lastName}".Trim(), email, roles, permissions);
            }
        }
        catch
        {
            // Silently continue if backend is not yet started
        }

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        
        base.OnExit(e);
    }

    private bool _isHandlingException;

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        if (_isHandlingException) return;
        _isHandlingException = true;
        
        System.IO.File.WriteAllText("crash.log", e.Exception.ToString());
        MessageBox.Show($"حدث خطأ غير متوقع:\n{e.Exception.Message}", "خطأ بالنظام", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
        
        _isHandlingException = false;
    }
}
