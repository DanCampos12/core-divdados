using Core.Divdados.Domain.UserContext.Entities;
using System.Security.Claims;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Core.Divdados.Domain.UserContext.Results;
using Newtonsoft.Json;
using System.Linq;

namespace Core.Divdados.Domain.UserContext.Services;

public sealed class AuthService
{
    private readonly JwtBearer _jwtBearer;

    public AuthService(JwtBearer jwtBearer)
    {
        _jwtBearer = jwtBearer;
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtBearer.SecretKey);
        var userCustomClaims = new Claim[]
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("userEmail", user.Email),
            new Claim("userName", $"{user.Name} {user.Surname}")
        };
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
        {
            Issuer = _jwtBearer.ValidIssuer,
            Audience = _jwtBearer.ValidAudience,
            Expires = DateTime.UtcNow.AddHours(1),
            Subject = new ClaimsIdentity(userCustomClaims),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        });

        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(Guid userId, string idToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtBearer.SecretKey);
            var validatedToken = tokenHandler.ValidateToken(idToken, new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtBearer.ValidIssuer,
                ValidAudience = _jwtBearer.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out var _);

            var claimsUserId = validatedToken.Claims.FirstOrDefault(x => x.Type.Equals("userId")).Value;
            return claimsUserId.Equals(userId.ToString());
        } 
        catch
        {
            return false;
        }
    }
}
