using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
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
        var httpRoute = context.Request.Path.Value;
        var registerPaths = new string[] { "/v1/users", "/v1/users/auth/sign-in" };
        Guid? userId = null;

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

        if (Guid.TryParse(httpRoute.Split("/")[3], out var userIdParsed))
            userId = userIdParsed;

        if (!_authService.ValidateToken(headerAuthorization.Replace("Bearer ", ""), userId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token inválido");
            return;
        }

        await _requestDelegate(context);
    }
}