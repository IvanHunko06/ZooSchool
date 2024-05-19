namespace SchoolServer.DataAccess.SQLServer.Models;

public class RoleEntity
{
    public int Id { get; set; } = -1;
    public string RoleName { get; set; } = string.Empty;
    public List<UserEntity> Users { get; set; } = [];
    public List<PermissionEntity> Permissions { get; set; } = [];

}
