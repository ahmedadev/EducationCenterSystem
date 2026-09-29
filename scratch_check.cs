using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EducationCenterSystem.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=EducationCenterSystem;Username=postgres;Password=postgres");
        using var context = new ApplicationDbContext(optionsBuilder.Options);
        
        int parentsCount = context.Parents.Count();
        Console.WriteLine($"Total Parents: {parentsCount}");
    }
}
