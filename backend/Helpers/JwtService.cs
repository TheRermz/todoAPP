namespace todoApp.Helpers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using todoApp.Models;


public class JwtService
{
    public string GenerateJwtToken(User user)
    {
        var key = Environment.GetEnvironmentVariable("JWT_SECRET");
        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("Variável de ambiente JWT_SECRET não foi definida.");
        }
        var keyToBytes = Encoding.ASCII.GetBytes(key);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
        };
        var securityKey = new SymmetricSecurityKey(keyToBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(tokenDescriptor);
        return tokenString;


    }
}
