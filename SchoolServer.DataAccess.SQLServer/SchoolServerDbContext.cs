
using Microsoft.EntityFrameworkCore;
using SchoolServer.DataAccess.SQLServer.Models;
using SchoolServer.DataAccess.SQLServer.Configurations;
using Microsoft.Extensions.Options;
namespace SchoolServer.DataAccess.SQLServer;

public class SchoolServerDbContext : DbContext
{
    private readonly IOptions<AuthorizationOptions> authOptions;

    public SchoolServerDbContext(DbContextOptions<SchoolServerDbContext> options, IOptions<AuthorizationOptions> authOptions) 
        : base(options)
    {
        this.authOptions = authOptions;
    }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<UserRoleEntity> UsersRoles { get; set; }
    public DbSet<RolePermissionEntity> RolesPermissions { get; set; }
    public DbSet<LessonEntity> Lessons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new PermisionConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration(authOptions.Value));
        modelBuilder.ApplyConfiguration(new UsersRolesConfiguration());
        modelBuilder.ApplyConfiguration(new LessonConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
