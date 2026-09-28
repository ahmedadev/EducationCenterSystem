using EducationCenterSystem.Application.Common.Interfaces;
using EducationCenterSystem.Domain.Repositories;
using EducationCenterSystem.Infrastructure.Repositories;
using EducationCenterSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EducationCenterSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()));

        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IEducationalGroupRepository, EducationalGroupRepository>();
        services.AddScoped<IGroupSessionRepository, GroupSessionRepository>();
        services.AddScoped<IAttendanceRecordRepository, AttendanceRecordRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.Configure<Authentication.JwtSettings>(configuration.GetSection(Authentication.JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
