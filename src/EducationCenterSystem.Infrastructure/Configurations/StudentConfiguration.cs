using EducationCenterSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationCenterSystem.Infrastructure.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.LastName).HasMaxLength(100).IsRequired();

        builder.OwnsOne(s => s.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();
            emailBuilder.HasIndex(e => e.Value).IsUnique();
        });

        builder.OwnsOne(s => s.PhoneNumber, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(s => s.ParentPhoneNumber, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasColumnName("ParentPhoneNumber")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(s => s.NationalId).HasMaxLength(14);
        builder.HasIndex(s => s.NationalId)
            .IsUnique()
            .HasFilter("\"NationalId\" IS NOT NULL");

        builder.Property(s => s.StudentCode).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => s.StudentCode).IsUnique();

        builder.Property(s => s.GradeLevel).HasMaxLength(50).IsRequired();
        builder.Property(s => s.SchoolName).HasMaxLength(200);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.Notes).HasMaxLength(1000);
    }
}
