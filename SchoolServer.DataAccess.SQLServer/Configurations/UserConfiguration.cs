﻿using Microsoft.EntityFrameworkCore;
using SchoolServer.DataAccess.SQLServer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace SchoolServer.DataAccess.SQLServer.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(500)
            .IsUnicode(false);

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<UserRoleEntity>(
                l => l.HasOne<RoleEntity>().WithMany().HasForeignKey(r => r.RoleId),
                r => r.HasOne<UserEntity>().WithMany().HasForeignKey(u => u.UserId)
            );

        builder.HasMany(u=>u.TestAttempts)
            .WithOne(a=>a.User)
            .HasForeignKey(a=>a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasIndex(x => x.Username).IsUnique();

        builder.HasMany(u => u.Favorites)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);
    }
}
