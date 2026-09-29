using EducationCenterSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationCenterSystem.Infrastructure.Configurations;

internal sealed class ParentConfiguration : IEntityTypeConfiguration<Parent>
{
    public void Configure(EntityTypeBuilder<Parent> builder)
    {
        builder.ToTable("parents");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.SecondName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.ThirdName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(p => p.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(255);
            emailBuilder.HasIndex(e => e.Value).IsUnique();
        });

        builder.OwnsOne(p => p.PhoneNumber, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasColumnName("phone_number")
                .HasMaxLength(20)
                .IsRequired();
            phoneBuilder.HasIndex(p => p.Value).IsUnique();
        });

        builder.Property(p => p.NationalId)
            .HasMaxLength(20);

        builder.Property(p => p.Job)
            .HasMaxLength(200);

        builder.Property(p => p.Address)
            .HasMaxLength(500);

        builder.Property(p => p.Notes)
            .HasMaxLength(1000);

        builder.Property(p => p.RegisteredOnUtc)
            .IsRequired();

        builder.HasMany(p => p.Children)
            .WithOne(s => s.Parent)
            .HasForeignKey(s => s.ParentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
