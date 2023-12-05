using Core.Divdados.Domain.UserContext.Entities;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Core.Divdados.Domain.UserContext.Services;

public sealed class AuthService
{
    private readonly JwtBearer _jwtBearer;

    public AuthService(JwtBearer jwtBearer)
    {
        _jwtBearer = jwtBearer;
    }

    public string GenerateToken(User user, DateTime expires)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtBearer.SecretKey);
        var userCustomClaims = new Claim[]
        {
            new Claim("userId", user.Id.ToString()),
            new Claim("userEmail", user.Email),
            new Claim("userName", user.Name)
        };
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
        {
            Issuer = _jwtBearer.ValidIssuer,
            Audience = _jwtBearer.ValidAudience,
            Expires = expires,
            Subject = new ClaimsIdentity(userCustomClaims),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        });

        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string idToken, Guid? userId = null)
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

            if (userId is null) return true;

            var claimsUserId = validatedToken.Claims.FirstOrDefault(x => x.Type.Equals("userId")).Value;
            return claimsUserId.Equals(userId.ToString());
        } 
        catch
        {
            return false;
        }
    }

    public Guid GetUserId (string idToken)
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

        return Guid.Parse(validatedToken.Claims.FirstOrDefault(x => x.Type.Equals("userId")).Value);
    }

    public static string EncryptPassword(string password)
    {
        var randomHash = new byte[16];
        RandomNumberGenerator.Fill(randomHash);
        var encryptedPassword = new Rfc2898DeriveBytes(password, randomHash, 10000, HashAlgorithmName.SHA256);
        var encryptedPasswordHash = encryptedPassword.GetBytes(32);
        var hashedPassword = $"{Convert.ToBase64String(encryptedPasswordHash)}.{Convert.ToBase64String(randomHash)}";
        return hashedPassword;
    }

    public static bool ValidatePassword(string password, string hashedPassword)
    {
        var hashComposition = hashedPassword.Split(".");
        var encryptedPasswordHash = Convert.FromBase64String(hashComposition[0]);
        var randomGeneratedHash = Convert.FromBase64String(hashComposition[1]);
        var encryptedEnteredPassword = new Rfc2898DeriveBytes(password, randomGeneratedHash, 10000, HashAlgorithmName.SHA256);
        var encryptedEnteredPasswordHash = encryptedEnteredPassword.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(encryptedPasswordHash, encryptedEnteredPasswordHash);
    }
}
