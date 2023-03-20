using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Divdados.Api.Configurations;

public static class MediatRConfiguration
{
    public static void AddMediator(this IServiceCollection services) =>
        services.AddMediatR(Assembly.Load("Core.Divdados.Domain"));
}
