using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EducationCenterSystem.Presentation.WinForms.Services;
using EducationCenterSystem.Presentation.WinForms.Services.Abstractions;
using EducationCenterSystem.Presentation.WinForms.Views;

namespace EducationCenterSystem.Presentation.WinForms;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core Services
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddTransient<AuthenticatedHttpClientHandler>();
        
        // API Services
        services.AddTransient<IAuthApiService, AuthApiService>();
        services.AddTransient<ITeacherApiService, TeacherApiService>();
        services.AddTransient<IStudentApiService, StudentApiService>();
        services.AddTransient<IEducationalGroupApiService, EducationalGroupApiService>();
        services.AddTransient<ICourseApiService, CourseApiService>();
        services.AddTransient<IAttendanceApiService, AttendanceApiService>();
        services.AddTransient<IUserApiService, UserApiService>();
        services.AddTransient<IRoleApiService, RoleApiService>();

        // HTTP Client configuration
        services.AddHttpClient(string.Empty, client =>
        {
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5145/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);
        })
        .AddHttpMessageHandler<AuthenticatedHttpClientHandler>()
        .AddStandardResilienceHandler();

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

        return services;
    }
}
