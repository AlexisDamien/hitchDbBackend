using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    public AuthService(IConfiguration config) => _config = config;

public string GenerateToken(User user)
{
    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

    var claims = new Dictionary<string, object>
    {
        [ClaimTypes.NameIdentifier] = user.Id.ToString(),
        [ClaimTypes.Email] = user.Email
    };

    var descriptor = new SecurityTokenDescriptor
    {
        Claims = claims,
        Expires = DateTime.UtcNow.AddDays(7),
        Issuer = _config["Jwt:Issuer"],
        Audience = _config["Jwt:Audience"],
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
    };

    var handler = new JsonWebTokenHandler();
    return handler.CreateToken(descriptor);
}
}