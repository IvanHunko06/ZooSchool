using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SchoolServer.Application.Exceptions;
using SchoolServer.Application.Interfaces.Repositories;
using SchoolServer.Core.Enums;

namespace SchoolServer.Infrastructure.Authentification;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirment>
{
    private readonly JWTOptions jWToptions;
    private readonly IUsersRepository usersRepository;

    public PermissionAuthorizationHandler(IOptions<JWTOptions> jWToptions, IUsersRepository usersRepository)
    {
        this.jWToptions = jWToptions.Value;
        this.usersRepository = usersRepository;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirment requirement)
    {

        var userIdClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == jWToptions.UserIdClaim);
        Guid userId;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out userId))
            return;

        try
        {
            var permissions = await usersRepository.GetUserPermissions(userId);
            if (permissions.Contains(requirement.Permission))
                context.Succeed(requirement);
        }
        catch (UserNotFoundException)
        {
            return;
        }

    }
}
