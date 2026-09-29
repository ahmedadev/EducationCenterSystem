using EducationCenterSystem.Domain.Entities;
using EducationCenterSystem.Domain.Enums;
using EducationCenterSystem.Domain.ValueObjects;
using EducationCenterSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EducationCenterSystem.Api;

public static class SeedDataExtensions
{
    public const string DefaultAdminEmail = "admin@educationcenter.com";

    private static readonly string[] _maleNames = { "أحمد", "محمد", "محمود", "يوسف", "عمر", "علي", "إبراهيم", "كريم", "حسن", "حسين", "خالد", "طارق", "زياد", "مصطفى", "حمزة", "هشام", "عصام", "أشرف", "سامح", "مدحت", "شريف", "حازم", "ياسر", "عمرو", "تامر", "مجدي", "وليد", "وائل", "هاني", "أكرم", "بهاء", "رامي", "شادي", "علاء", "عماد", "جمال", "كمال", "سعيد", "صلاح" };
    private static readonly string[] _femaleNames = { "سارة", "مريم", "نور", "فاطمة", "سلمى", "آية", "حبيبة", "ملك", "رنا", "ياسمين", "شهد", "فريدة", "جنا", "ندى", "هاجر", "منى", "ريهام", "هبة", "إيمان", "نهى", "سحر", "عبير", "نادية", "حنان", "وفاء", "أمل", "داليا", "شيماء", "رانيا", "سماح", "أسماء", "دعاء", "زينب", "هند", "مي", "مروة", "رحاب", "نجلاء", "شيرين", "بسمة" };
    private static readonly string[] _lastNames = { "علاء", "خالد", "شريف", "السيد", "طارق", "حسام", "مصطفى", "فتحي", "عادل", "إيهاب", "رضوان", "توفيق", "عبد الرحمن", "الشافعي", "البدري", "المنشاوي", "الشناوي", "عبد الفتاح", "المهدي", "الصاوي", "البيومي", "السعدني", "الجوهري", "المغربي", "الغندور", "النحاس", "الفقي", "الباز", "القاضي", "زهران", "دسوقي", "السعيد", "رمضان", "مختار", "رياض", "منصور", "شاهين", "سليمان", "شفيق", "عبد الله", "حافظ", "فاروق", "عثمان", "عامر", "صادق" };
    private static readonly string[] _grades = { "الصف الأول الإعدادي", "الصف الثاني الإعدادي", "الصف الثالث الإعدادي", "الصف الأول الثانوي", "الصف الثاني الثانوي", "الصف الثالث الثانوي" };
    private static readonly string[] _schools = { "مدرسة المستقبل لغات", "مدرسة المتفوقين STEM", "مدرسة الأورمان الثانوية", "مدرسة النور الخاصة", "مدرسة النيل المصرية", "مدرسة السلام الرسمية" };
    private static readonly string[] _addresses = { "القاهرة، المعادي", "الجيزة، المهندسين", "الإسكندرية، لوران", "القليوبية، شبرا الخيمة", "المنصورة، حي الجامعة", "الشرقية، القومية", "الغربية، طنطا", "القاهرة، مدينة نصر", "الجيزة، الدقي", "الإسكندرية، سموحة", "القليوبية، بنها", "المنصورة، المشاية", "الشرقية، الزقازيق", "طنطا، النحاس" };
    private static readonly string[] _subjects = { "اللغة العربية", "اللغة الإنجليزية", "اللغة الفرنسية", "الرياضيات", "الفيزياء", "الكيمياء", "الأحياء", "التاريخ", "الجغرافيا", "الفلسفة والمنطق", "الجيولوجيا", "الحاسب الآلي" };
    private static readonly string[] _qualifications = { "بكالوريوس تربية", "ماجستير مناهج وطرق تدريس", "دبلومة تربوية عامة", "بكالوريوس علوم ورياضيات", "ليسانس آداب وتربية", "دكتوراه في المناهج التعليمية" };
    private static readonly string[] _notesList = { "معلم أول خبير ومعتمد", "رئيس قسم المادة للمرحلة الثانوية", "حاصل على درع التميز التعليمي", "معلم معتمد للمرحلتين الإعدادية والثانوية", "منسق تدريب المعلمين الجدد" };

    public static async Task SeedDataAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        int currentCount = await dbContext.Students.CountAsync();
        const int targetCount = 1000;

        if (currentCount >= targetCount)
        {
            return;
        }


        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        const int batchSize = 5000;
        var batch = new List<Student>(batchSize);

        for (int i = currentCount + 1; i <= targetCount; i++)
        {
            bool isFemale = (i % 2 == 0);
            Gender gender = isFemale ? Gender.Female : Gender.Male;
            string firstName = isFemale 
                ? _femaleNames[(i / 2) % _femaleNames.Length] 
                : _maleNames[(i / 2) % _maleNames.Length];
            string lastName = _lastNames[i % _lastNames.Length];

            var emailResult = Email.Create($"student{i:D6}@education.eg");
            var phoneResult = PhoneNumber.Create($"010{i:D8}");
            var parentPhoneResult = PhoneNumber.Create($"011{i:D8}");

            int year = 2005 + (i % 4);
            int month = 1 + (i % 12);
            int day = 1 + (i % 28);
            DateTime dob = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            string nationalId = $"3{i:D13}"; // Exactly 14 digits, unique
            string grade = _grades[i % _grades.Length];
            string code = $"STU-{i:D6}";
            string school = _schools[i % _schools.Length];
            string address = _addresses[i % _addresses.Length];
            string? notes = (i % 3 == 0) ? "طالب متميز ومتفوق" : (i % 5 == 0 ? "يحتاج لمتابعة في الواجبات" : null);

            string secondName = _maleNames[(i * 7) % _maleNames.Length];
            string thirdName = _maleNames[(i * 13) % _maleNames.Length];

            var studentResult = Student.Register(
                firstName,
                secondName,
                thirdName,
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

    public static async Task SeedTeachersDataAsync(IServiceProvider services, int targetCount = 70)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


        var existingTeachers = await dbContext.Teachers.Where(t => t.SecondName == "" || t.ThirdName == "").ToListAsync();
        if (existingTeachers.Any())
        {
            for (int i = 0; i < existingTeachers.Count; i++)
            {
                var t = existingTeachers[i];
                string secondName = _maleNames[(i * 2) % _maleNames.Length];
                string thirdName = _maleNames[(i * 3) % _maleNames.Length];
                t.UpdateProfile(t.FirstName, secondName, thirdName, t.LastName, t.Email, t.PhoneNumber, t.DateOfBirth, t.NationalId, t.TeacherCode, t.Subject, t.Qualification, t.Gender, t.Address, t.Notes);
            }
            await dbContext.SaveChangesAsync();
        }

        int currentCount = await dbContext.Teachers.CountAsync();
        if (currentCount >= targetCount)
        {
            return;
        }



        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        const int batchSize = 500;
        var batch = new List<Teacher>(batchSize);

        for (int i = currentCount + 1; i <= targetCount; i++)
        {
            // First 500 are male, next 200 are female
            bool isMale = i <= 500;
            Gender gender = isMale ? Gender.Male : Gender.Female;
            
            string firstName = isMale
                ? _maleNames[i % _maleNames.Length]
                : _femaleNames[i % _femaleNames.Length];
            
            // Second and Third names are usually male names for both genders in Egypt
            string secondName = _maleNames[(i * 2) % _maleNames.Length];
            string thirdName = _maleNames[(i * 3) % _maleNames.Length];
            string lastName = _lastNames[i % _lastNames.Length];

            string uniqueStr = Guid.NewGuid().ToString()[..4];
            var emailResult = Email.Create($"teacher{i:D4}_{uniqueStr}@education.eg");
            var phoneResult = PhoneNumber.Create($"012{i:D8}");

            int birthYear = 1970 + (i % 25);
            int birthMonth = 1 + (i % 12);
            int birthDay = 1 + (i % 28);
            DateTime dob = new DateTime(birthYear, birthMonth, birthDay, 0, 0, 0, DateTimeKind.Utc);

            string randomSuffix = Guid.NewGuid().ToString("N")[..4].ToUpper();
            int rndPart = Random.Shared.Next(1000, 9999);
            string nationalId = $"2{(birthYear % 100):D2}{birthMonth:D2}{birthDay:D2}{rndPart}{i:D3}";
            if (nationalId.Length > 14) nationalId = nationalId[..14];

            string teacherCode = $"TCH-{i:D4}-{randomSuffix}";
            string subject = _subjects[i % _subjects.Length];
            string qualification = _qualifications[i % _qualifications.Length];
            string address = _addresses[i % _addresses.Length];
            string notes = _notesList[i % _notesList.Length];

            var teacherResult = Teacher.Register(
                firstName,
                secondName,
                thirdName,
                lastName,
                emailResult.Value,
                phoneResult.Value,
                dob,
                nationalId,
                teacherCode,
                subject,
                qualification,
                gender,
                address,
                notes);

            if (!teacherResult.IsError)
            {
                batch.Add(teacherResult.Value);
            }

            if (batch.Count == batchSize)
            {
                await dbContext.Teachers.AddRangeAsync(batch);
                await dbContext.SaveChangesAsync();
                dbContext.ChangeTracker.Clear();
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await dbContext.Teachers.AddRangeAsync(batch);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();
            batch.Clear();
        }

        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public static async Task SeedTeachersAsync(this WebApplication app, int targetCount = 70)
    {
        using var scope = app.Services.CreateScope();
        await SeedTeachersDataAsync(scope.ServiceProvider, targetCount);
    }

    public static async Task SeedStudentsAsync(this WebApplication app)
    {
        await SeedDataAsync(app.Services);
    }

    public static async Task SeedDefaultAdminUserAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<Application.Common.Interfaces.IPasswordHasher>();

        var adminUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.Value == DefaultAdminEmail);
        
        if (adminUser == null)
        {
            var adminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole is not null)
            {
                // 🔒 Security: No hardcoded fallback password.
                // Set AdminPassword in environment variables or User Secrets.
                var rawPassword = app.Configuration["AdminPassword"]
                    ?? throw new InvalidOperationException(
                        "AdminPassword configuration key is required. " +
                        "Set it via environment variable or User Secrets. " +
                        "Example: dotnet user-secrets set \"AdminPassword\" \"<your-strong-password>\"");

                var emailResult = Email.Create(DefaultAdminEmail);
                string passwordHash = passwordHasher.Hash(rawPassword);

                var adminUserResult = User.Create(
                    "System",
                    "Admin",
                    emailResult.Value,
                    passwordHash);

                if (!adminUserResult.IsError)
                {
                    adminUser = adminUserResult.Value;
                    adminUser.AssignRole(adminRole.Id);

                    await dbContext.Users.AddAsync(adminUser);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        else
        {
            // Force update password for local development sync
            var rawPassword = app.Configuration["AdminPassword"];
            if (!string.IsNullOrEmpty(rawPassword))
            {
                adminUser.ChangePassword(passwordHasher.Hash(rawPassword));
                await dbContext.SaveChangesAsync();
            }
        }
    }

    public static async Task SeedParentsDataAsync(IServiceProvider services, int targetCount = 500)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        int currentCount = await dbContext.Parents.CountAsync();
        if (currentCount >= targetCount)
        {
            return;
        }

        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        var studentsToLink = await dbContext.Students
            .Where(s => s.ParentId == null)
            .OrderBy(s => s.Id)
            .Take(960)
            .ToListAsync();

        int studentIndex = 0;
        var batch = new List<Parent>(500);

        for (int i = currentCount + 1; i <= targetCount; i++)
        {
            string firstName = _maleNames[i % _maleNames.Length];
            string secondName = _maleNames[(i * 2) % _maleNames.Length];
            string thirdName = _maleNames[(i * 3) % _maleNames.Length];
            string lastName = _lastNames[i % _lastNames.Length];

            string uniqueStr = Guid.NewGuid().ToString()[..4];
            var emailResult = Email.Create($"parent{i:D4}_{uniqueStr}@education.eg");
            var phoneResult = PhoneNumber.Create($"011{i:D8}");

            int birthYear = 1960 + (i % 20);
            int birthMonth = 1 + (i % 12);
            int birthDay = 1 + (i % 28);
            int rndPart = Random.Shared.Next(1000, 9999);
            string nationalId = $"2{(birthYear % 100):D2}{birthMonth:D2}{birthDay:D2}{rndPart}{i:D3}";
            if (nationalId.Length > 14) nationalId = nationalId[..14];

            string job = "موظف";
            string address = _addresses[i % _addresses.Length];
            string notes = "تم إضافته عشوائياً";

            var parentResult = Parent.Register(
                firstName,
                secondName,
                thirdName,
                lastName,
                emailResult.Value,
                phoneResult.Value,
                nationalId,
                job,
                address,
                notes);

            if (!parentResult.IsError)
            {
                var parent = parentResult.Value;

                int targetLink = 1;
                if (i <= 20) targetLink = 4;
                else if (i <= 70) targetLink = 3;
                else if (i <= 370) targetLink = 2;

                for (int j = 0; j < targetLink; j++)
                {
                    if (studentIndex < studentsToLink.Count)
                    {
                        var st = studentsToLink[studentIndex++];
                        typeof(Student).GetProperty("ParentId")?.SetValue(st, parent.Id);
                    }
                }

                batch.Add(parent);
            }
        }

        if (batch.Count > 0)
        {
            await dbContext.Parents.AddRangeAsync(batch);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();
        }

        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public static async Task SeedParentsAsync(this WebApplication app)
    {
        await SeedParentsDataAsync(app.Services);
    }
}
