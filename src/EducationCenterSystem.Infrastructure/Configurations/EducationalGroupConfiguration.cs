using EducationCenterSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationCenterSystem.Infrastructure.Configurations;

public sealed class EducationalGroupConfiguration : IEntityTypeConfiguration<EducationalGroup>
{
    public void Configure(EntityTypeBuilder<EducationalGroup> builder)
    {
        builder.ToTable("EducationalGroups");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.MaxCapacity)
            .IsRequired();

        builder.Property(g => g.MonthlyFee)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(g => g.ScheduleDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(g => g.Status)
            .IsRequired();

        builder.HasOne(g => g.Course)
            .WithMany(c => c.Groups)
            .HasForeignKey(g => g.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Teacher)
            .WithMany(t => t.Groups)
            .HasForeignKey(g => g.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(g => g.Enrollments)
            .WithOne(e => e.EducationalGroup)
            .HasForeignKey(e => e.EducationalGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
