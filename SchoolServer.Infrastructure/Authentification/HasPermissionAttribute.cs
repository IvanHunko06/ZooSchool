using Microsoft.AspNetCore.Authorization;
using SchoolServer.Core.Enums;

namespace SchoolServer.Infrastructure.Authentification;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permission permission) 
        :base(policy: permission.ToString())
    {
        
    }
}
