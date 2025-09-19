using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entites;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
  public string CreateToken(AppUser user)
  {

    var tokenkey = config["TokenKey"] ?? throw new Exception("invalid token key");

    if (tokenkey.Length < 64)
      throw new Exception("your token key needs to be >= 64 characters");
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenkey));

    var claims = new List<Claim>
      {
        new(ClaimTypes.Email,user.Email),
        new(ClaimTypes.NameIdentifier,user.Id)
      };

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

    var tokenDesciptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddDays(7),
      SigningCredentials = creds
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDesciptor);

    return tokenHandler.WriteToken(token);
  }
}
