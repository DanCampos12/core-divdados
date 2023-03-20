using Core.Divdados.Shared.Uow;
using Core.Divdados.Infra.SQL.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Infra.SQL.Repositories;
using Core.Divdados.Domain.UserContext.Commands.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;

namespace Core.Divdados.Api.Configurations;

public static class DomainConfiguration
{
    public static void AddDomainDependencies(this IServiceCollection services, SettingsModel settingsModel)
    {
        services.AddProvider(settingsModel);
        services.AddTransient<IUow, Uow>();
        services.AddRepositories();
    }

    public static void AddProvider(this IServiceCollection services, SettingsModel settingsModel)
    {
        services.AddScoped<string>(serviceProvider => settingsModel.ConnectionString);
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
    }
}
