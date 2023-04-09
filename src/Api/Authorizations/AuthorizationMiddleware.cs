using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Divdados.Api.Authorizations;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly string _clientId;
    private readonly AuthService _authService;

    public AuthorizationMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
    {
        _requestDelegate = requestDelegate;
        _clientId = configuration.GetSection("Settings").Get<SettingsModel>().ClientId;
        _authService = new AuthService(configuration.GetSection("Settings").Get<Settings>().JwtBearer);
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        var headerClientId = context.Request.Headers["x-client-id"].ToString();
        var headerAuthorization = context.Request.Headers["authorization"].ToString();
        var httpMethod = context.Request.Method.ToString();
        var httpPath = context.Request.Path.ToString();
        var registerPaths = new string[] { 
            "/v1/users", 
            "/v1/users/auth/sign-in", 
            "/v1/users/auth/refresh-token" };

        if (httpMethod.Equals("OPTIONS"))
        {
            context.Response.StatusCode = 200;
            await _requestDelegate(context);
            return;
        }

        if (!_clientId.Equals(headerClientId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("x-client-id inválido");
            return;
        }

        if (registerPaths.Contains(httpPath.ToString()) && httpMethod.Equals("POST"))
        {
            await _requestDelegate(context);
            return;
        }

        if (!_authService.ValidateToken(headerAuthorization.Replace("Bearer ", "")))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token inválido");
            return;
        }

        await _requestDelegate(context);
    }
}