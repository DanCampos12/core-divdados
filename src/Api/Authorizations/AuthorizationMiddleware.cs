using Core.Divdados.Domain.UserContext.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Core.Divdados.Api.Authorizations;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly string _clientId;
    private readonly JwtBearer _jwtBearer;

    public AuthorizationMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
    {
        _requestDelegate = requestDelegate;
        _clientId = configuration.GetSection("Settings").Get<SettingsModel>().ClientId;
        _jwtBearer = configuration.GetSection("Settings").Get<SettingsModel>().JwtBearer;
    }

    public async Task Invoke(HttpContext context)
    {
        var headerClientId = context.Request.Headers["x-client-id"].ToString();
        var headerAuthorization = context.Request.Headers["authorization"].ToString();
        var httpMethod = context.Request.Method.ToString();
        var httpPath = context.Request.Path.ToString();
        var validPathsWithoutToken = new string[] { 
            "/v1/users", 
            "/v1/users/auth/sign-in", 
            "/v1/users/auth/refresh-token" };

        if (!_clientId.Equals(headerClientId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("x-client-id inválido");
            return;
        }

        if (validPathsWithoutToken.Contains(httpPath.ToString()) && httpMethod.Equals("POST"))
        {
            await _requestDelegate(context);
            return;
        }

        if (!ValidateToken(headerAuthorization.Replace("Bearer ", "")))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token inválido");
            return;
        }

        await _requestDelegate(context);
    }

    private bool ValidateToken(string idToken)
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

            return true;
        }
        catch
        {
            return false;
        }
    }
}