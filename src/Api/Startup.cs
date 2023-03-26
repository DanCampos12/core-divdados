using Core.Divdados.Api.Configurations;
using Core.Divdados.Infra.SQL.DataContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Text;

namespace Core.Divdados.Api;

public class Startup
{
    public readonly IConfigurationSection _section;
    private SettingsModel _settingsModel;

    public Startup(IConfiguration configuration)
    {
        _section = configuration.GetSection("Settings");
    }

    public void ConfigureServices(IServiceCollection services) 
    {
        _settingsModel = _section.Get<SettingsModel>();

        services.AddMvc(option => option.EnableEndpointRouting = false);
        services.AddMvc().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        });
        services.AddCors();
        services.AddHttpContextAccessor();
        services.Configure<SettingsModel>(_section);
        services.Configure<GzipCompressionProviderOptions>(opt => opt.Level = System.IO.Compression.CompressionLevel.Fastest);
        services.AddResponseCompression(opt => opt.Providers.Add<GzipCompressionProvider>());
        services.AddDomainDependencies(_settingsModel);
        services.AddSwagger();
        services.AddMediator();
        services.AddDbContext<UserDataContext>();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        app.UseMiddleware<Authorizations.AuthorizationMiddleware>();
        app.UseHttpsRedirection();
        app.UseResponseCompression();
        app.UseCors(builder => builder
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader());
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseMvc();

        var cultureInfo = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}
