using Microsoft.EntityFrameworkCore;
using SchoolServer.DataAccess.SQLServer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolServer.Core.Enums;

namespace SchoolServer.DataAccess.SQLServer.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RoleName)
            .IsRequired()
            .HasMaxLength(10)
            .IsUnicode(false);


        builder.HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity<RolePermissionEntity>(
                l => l.HasOne<PermissionEntity>().WithMany().HasForeignKey(p => p.PermissionId),
                r => r.HasOne<RoleEntity>().WithMany().HasForeignKey(r => r.RoleId)
            );

        var roles = Enum.GetValues<Role>().Select(r => new RoleEntity
        {
            Id = (int)r,
            RoleName = r.ToString()
        });

        builder.HasData(roles);
    }
}
