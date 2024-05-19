using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchoolServer.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SchoolServer.Application.Interfaces.Auth;
namespace SchoolServer.Infrastructure.Authentification;

public class JWTProvider : IJWTProvider
{
    JWTOptions options;
    public JWTProvider(IOptions<JWTOptions> options)
    {
        this.options = options.Value;
    }
    public string GenerateToken(User user)
    {
        Claim[] claims = [new(options.UserIdClaim, user.Id.ToString())];
        var signingCredentials = new SigningCredentials
            (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(options.ExpiresHours),
            claims: claims
            );
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }
    public Guid GetUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == options.UserIdClaim);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        throw new ArgumentException("Invalid token");

    }
}
