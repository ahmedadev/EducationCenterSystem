using EducationCenterSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationCenterSystem.Infrastructure.Configurations;

public sealed class GroupSessionConfiguration : IEntityTypeConfiguration<GroupSession>
{
    public void Configure(EntityTypeBuilder<GroupSession> builder)
    {
        builder.ToTable("GroupSessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SessionDate)
            .IsRequired();

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.HasOne(s => s.EducationalGroup)
            .WithMany()
            .HasForeignKey(s => s.EducationalGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.AttendanceRecords)
            .WithOne(a => a.GroupSession)
            .HasForeignKey(a => a.GroupSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
