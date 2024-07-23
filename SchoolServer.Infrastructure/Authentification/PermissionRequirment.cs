using Microsoft.AspNetCore.Authorization;
using SchoolServer.Core.Enums;

namespace SchoolServer.Infrastructure.Authentification;

public class PermissionRequirment : IAuthorizationRequirement
{

    public PermissionRequirment(Permission permissions)
    {
        Permission = permissions;
    }

    public PermissionRequirment(string permission)
    {
        Permission = (Permission)Permission.Parse(typeof(Permission), permission);
    }

    public Permission Permission { get; set; }
}
