using Microsoft.AspNetCore.Mvc;

namespace EducationCenterSystem.Api.Controllers;

[ApiController]
[Route("api/dev")]
// This controller will only be mapped/accessible in Development environment (enforced via Program.cs)
public class DevelopmentController : ControllerBase
{
    // Warning: Injecting WebApplication in a controller is a hack.
    // Better to inject IServiceProvider.
    private readonly IServiceProvider _serviceProvider;
    private readonly IWebHostEnvironment _env;

    public DevelopmentController(IServiceProvider serviceProvider, IWebHostEnvironment env)
    {
        _serviceProvider = serviceProvider;
        _env = env;
    }

    [HttpPost("seed-students")]
    public async Task<IActionResult> SeedStudents()
    {
        if (!_env.IsDevelopment())
        {
            return NotFound("This endpoint is only available in Development environment.");
        }
        try
        {
            await SeedDataExtensions.SeedDataAsync(_serviceProvider);
            return Ok(new { Message = "Successfully seeded 100k students." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
    
    [HttpPost("seed-teachers")]
    public async Task<IActionResult> SeedTeachers([FromQuery] int count = 1000)
    {
        if (!_env.IsDevelopment())
        {
            return NotFound("This endpoint is only available in Development environment.");
        }
        try
        {
            await SeedDataExtensions.SeedTeachersDataAsync(_serviceProvider, count);
            return Ok(new { Message = $"Successfully seeded {count} teachers." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
