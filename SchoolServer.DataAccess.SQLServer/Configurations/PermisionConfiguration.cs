using Microsoft.EntityFrameworkCore;
using SchoolServer.DataAccess.SQLServer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolServer.Core.Enums;
namespace SchoolServer.DataAccess.SQLServer.Configurations;

public class PermisionConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.ToTable("Permisions");
        builder.HasKey(x => x.Id);

        var permissions = Enum.GetValues<Permission>().Select(p => new PermissionEntity
        {
            Id = (int)p,
            Name = p.ToString(),
        });
        builder.HasData(permissions);

    }
}

