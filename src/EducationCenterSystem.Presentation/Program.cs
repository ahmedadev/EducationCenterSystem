using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;

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
                services.AddPresentationServices(context.Configuration);
            })
            .Build();

        Application.ThreadException += (sender, args) =>
        {
            var dialog = host.Services.GetService<IDialogService>();
            dialog?.ShowError($"حدث خطأ غير متوقع: {args.Exception.Message}", "خطأ في النظام");
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                var dialog = host.Services.GetService<IDialogService>();
                dialog?.ShowError($"انهيار في النظام: {ex.Message}", "خطأ حرج");
            }
        };

        var mainForm = host.Services.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }
}