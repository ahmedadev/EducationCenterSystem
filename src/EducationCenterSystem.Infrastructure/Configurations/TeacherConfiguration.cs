using EducationCenterSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationCenterSystem.Infrastructure.Configurations;

internal sealed class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(t => t.LastName).HasMaxLength(100).IsRequired();

        builder.OwnsOne(t => t.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(255)
                .IsRequired();
            emailBuilder.HasIndex(e => e.Value).IsUnique();
        });

        builder.OwnsOne(t => t.PhoneNumber, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(t => t.NationalId).HasMaxLength(14);
        builder.HasIndex(t => t.NationalId)
            .IsUnique()
            .HasFilter("\"NationalId\" IS NOT NULL");

        builder.Property(t => t.TeacherCode).HasMaxLength(50).IsRequired();
        builder.HasIndex(t => t.TeacherCode).IsUnique();

        builder.Property(t => t.Subject).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Qualification).HasMaxLength(200);
        builder.Property(t => t.Address).HasMaxLength(500);
        builder.Property(t => t.Notes).HasMaxLength(1000);
    }
}
