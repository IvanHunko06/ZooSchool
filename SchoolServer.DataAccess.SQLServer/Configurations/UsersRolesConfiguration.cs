using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolServer.Core.Enums;
using SchoolServer.DataAccess.SQLServer.Models;

namespace SchoolServer.DataAccess.SQLServer.Configurations;

public class UsersRolesConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.ToTable("UsersRoles");
        builder.HasKey(r => new {r.UserId, r.RoleId});
    }
}
