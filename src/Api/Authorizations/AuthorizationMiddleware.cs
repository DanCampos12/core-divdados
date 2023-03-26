using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Core.Divdados.Api.Authorizations;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly string _clientId;

    public AuthorizationMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
    {
        _requestDelegate = requestDelegate;
        _clientId = configuration.GetSection("Settings").Get<SettingsModel>().ClientId;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("x-client-id", out var headerClientId) || !_clientId.Equals(headerClientId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("x-client-id inválido");
            return;
        }

        await _requestDelegate(context);
    }
}