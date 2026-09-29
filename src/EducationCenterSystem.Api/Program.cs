using System.Text;
using System.Threading.RateLimiting;
using EducationCenterSystem.Api;
using EducationCenterSystem.Api.Authentication;
using EducationCenterSystem.Application;
using EducationCenterSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);



builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT Support
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
    options.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.ActionDescriptor.RouteValues["action"]}");
    options.ResolveConflictingActions(a => a.First());
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Education Center System API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    var securityScheme = new OpenApiSecuritySchemeReference("Bearer");
    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        [securityScheme] = []
    });
});

builder.Services.AddProblemDetails();

// CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secret = jwtSettings["Secret"];

if (string.IsNullOrEmpty(secret) || secret == "YOUR_JWT_SECRET_HERE")
{
    throw new InvalidOperationException("JWT Secret is missing. Please set it via environment variables or user-secrets.");
}

// 🔒 Security guard: AdminPassword must be explicitly set in production.
// Run: dotnet user-secrets set "AdminPassword" "<your-strong-password>"
if (!builder.Environment.IsDevelopment())
{
    var adminPassword = builder.Configuration["AdminPassword"];
    if (string.IsNullOrWhiteSpace(adminPassword))
    {
        throw new InvalidOperationException(
            "AdminPassword configuration key is required in non-Development environments. " +
            "Set it via an environment variable: AdminPassword=<your-strong-password>");
    }
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "EducationCenterSystem",
        ValidAudience = jwtSettings["Audience"] ?? "EducationCenterSystem",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});

// Authorization & Permission Provider
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 150;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 20;
    });
});

// Layer Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DefaultCorsPolicy");

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EducationCenterSystem.Infrastructure.ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Pre-warm database connection, Npgsql type catalog, and EF Core query cache
await app.WarmUpDatabaseAsync();

// Ensure default Admin user is seeded if database is fresh
await app.SeedDefaultAdminUserAsync();

// Ensure 70 Teachers are seeded if database is fresh
await app.SeedTeachersAsync();

// Ensure 1000 Students are seeded if database is fresh
await app.SeedStudentsAsync();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

// 🔒 Security: DevelopmentController is excluded from the route table in non-Development environments.
// This prevents accidental exposure of seed endpoints even if ASPNETCORE_ENVIRONMENT is misconfigured.
app.MapControllers()
   .RequireRateLimiting("fixed");

if (!app.Environment.IsDevelopment())
{
    // Explicitly block the dev-only routes at the routing level so they return 404 in production,
    // regardless of how DevelopmentController is registered.
    app.Map("/api/dev/{**slug}", () => Results.NotFound())
       .WithDisplayName("DevRouteBlocker");
}

app.Run();
