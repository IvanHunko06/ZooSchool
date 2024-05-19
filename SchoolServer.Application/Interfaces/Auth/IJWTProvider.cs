using SchoolServer.Core.Models;

namespace SchoolServer.Application.Interfaces.Auth;

public interface IJWTProvider
{
    string GenerateToken(User user);
    public Guid GetUserIdFromToken(string token);
}