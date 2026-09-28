using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.ValueObjects;
using EducationCenterSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Api;

public static class SeedDataExtensions
{
    public static async Task SeedDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        int currentCount = await dbContext.Students.CountAsync();
        const int targetCount = 100000;

        if (currentCount >= targetCount)
        {
            return;
        }

        var maleFirstNames = new[] { "أحمد", "محمد", "محمود", "يوسف", "عمر", "علي", "إبراهيم", "كريم", "حسن", "حسين", "خالد", "طارق", "زياد", "مصطفى", "حمزة" };
        var femaleFirstNames = new[] { "سارة", "مريم", "نور", "فاطمة", "سلمى", "آية", "حبيبة", "ملك", "رنا", "ياسمين", "شهد", "فريدة", "جنا", "ندى", "هاجر" };
        var lastNames = new[] { "علاء", "خالد", "شريف", "السيد", "طارق", "حسام", "مصطفى", "فتحي", "عادل", "إيهاب", "رضوان", "توفيق", "عبد الرحمن", "الشافعي", "البدري", "المنشاوي" };
        var grades = new[] { "الصف الأول الإعدادي", "الصف الثاني الإعدادي", "الصف الثالث الإعدادي", "الصف الأول الثانوي", "الصف الثاني الثانوي", "الصف الثالث الثانوي" };
        var schools = new[] { "مدرسة المستقبل لغات", "مدرسة المتفوقين STEM", "مدرسة الأورمان الثانوية", "مدرسة النور الخاصة", "مدرسة النيل المصرية", "مدرسة السلام الرسمية" };
        var cities = new[] { "القاهرة، مدينة نصر", "الجيزة، الدقي", "الإسكندرية، سموحة", "القليوبية، بنها", "المنصورة، المشاية", "الشرقية، الزقازيق", "طنطا، النحاس" };

        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        const int batchSize = 5000;
        var batch = new List<Student>(batchSize);

        for (int i = currentCount + 1; i <= targetCount; i++)
        {
            bool isFemale = (i % 2 == 0);
            Gender gender = isFemale ? Gender.Female : Gender.Male;
            string firstName = isFemale 
                ? femaleFirstNames[(i / 2) % femaleFirstNames.Length] 
                : maleFirstNames[(i / 2) % maleFirstNames.Length];
            string lastName = lastNames[i % lastNames.Length];

            var emailResult = Email.Create($"student{i:D6}@education.eg");
            var phoneResult = PhoneNumber.Create($"010{i:D8}");
            var parentPhoneResult = PhoneNumber.Create($"011{i:D8}");

            int year = 2005 + (i % 4);
            int month = 1 + (i % 12);
            int day = 1 + (i % 28);
            DateTime dob = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            string nationalId = $"3{i:D13}"; // Exactly 14 digits, unique
            string grade = grades[i % grades.Length];
            string code = $"STU-{i:D6}";
            string school = schools[i % schools.Length];
            string address = cities[i % cities.Length];
            string? notes = (i % 3 == 0) ? "طالب متميز ومتفوق" : (i % 5 == 0 ? "يحتاج لمتابعة في الواجبات" : null);

            var studentResult = Student.Register(
                firstName,
                lastName,
                emailResult.Value,
                phoneResult.Value,
                dob,
                nationalId,
                parentPhoneResult.Value,
                grade,
                code,
                school,
                gender,
                address,
                notes);

            if (!studentResult.IsError)
            {
                batch.Add(studentResult.Value);
            }

            if (batch.Count == batchSize)
            {
                await dbContext.Students.AddRangeAsync(batch);
                await dbContext.SaveChangesAsync();
                dbContext.ChangeTracker.Clear();
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await dbContext.Students.AddRangeAsync(batch);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();
            batch.Clear();
        }

        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public static async Task SeedDefaultAdminUserAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<Application.Common.Interfaces.IPasswordHasher>();

        if (!await dbContext.Users.AnyAsync())
        {
            var adminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole is not null)
            {
                var emailResult = Email.Create("admin@educationcenter.com");
                string passwordHash = passwordHasher.Hash(app.Configuration["AdminPassword"] ?? "Admin123456!");

                var adminUserResult = User.Create(
                    "System",
                    "Admin",
                    emailResult.Value,
                    passwordHash);

                if (!adminUserResult.IsError)
                {
                    var adminUser = adminUserResult.Value;
                    adminUser.AssignRole(adminRole.Id);

                    await dbContext.Users.AddAsync(adminUser);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
