namespace SchoolServer.DataAccess.SQLServer.Models;

public class UserEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<RoleEntity> Roles { get; set; }

}
