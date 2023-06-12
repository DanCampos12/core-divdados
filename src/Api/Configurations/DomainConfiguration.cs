using Core.Divdados.Api.Authorizations;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Infra.SQL.Repositories;
using Core.Divdados.Infra.SQL.Transactions;
using Core.Divdados.Shared.Uow;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddTransient<IAuthRepository, AuthRepository>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
    }
}
