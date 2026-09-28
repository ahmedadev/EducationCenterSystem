using EducationCenterSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationCenterSystem.Infrastructure.Configurations;

public sealed class StudentGroupConfiguration : IEntityTypeConfiguration<StudentGroup>
{
    public void Configure(EntityTypeBuilder<StudentGroup> builder)
    {
        builder.ToTable("StudentGroups");

        builder.HasKey(sg => sg.Id);

        builder.Property(sg => sg.EnrollmentDateUtc)
            .IsRequired();

        builder.Property(sg => sg.Status)
            .IsRequired();

        builder.HasOne(sg => sg.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(sg => sg.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sg => sg.EducationalGroup)
            .WithMany(g => g.Enrollments)
            .HasForeignKey(sg => sg.EducationalGroupId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Ensure student can't be enrolled in the same group twice
        builder.HasIndex(sg => new { sg.StudentId, sg.EducationalGroupId })
            .IsUnique();
    }
}
