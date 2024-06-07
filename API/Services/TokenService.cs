using System.Security.Claims;
using System.Text;
using Domain;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService
{
    public string CreateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("U+K7dV.n=tK)0XHSx4Q#j:TDqDN.c6qF*9Ukk.N,Q]m?tgZuKz1t[p-ppn!uWi5/"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = creds
        };

        var tokenHandler = new JsonWebTokenHandler();
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return token;
    }
}