using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Core.Divdados.Api.Authorizations;

public class AuthorizationAttribute : ActionFilterAttribute
{
    private readonly JwtBearer _jwtBearer;

    public AuthorizationAttribute(IConfiguration configuration)
    {
        _jwtBearer = configuration.GetSection("Settings").Get<SettingsModel>().JwtBearer;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("authorization", out var headerAuthorization) ||
            !ValidateToken(headerAuthorization.ToString()[7..]))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    public bool ValidateToken(string idToken)
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