using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace Core.Divdados.Api.Configurations;

public static class SwaggerConfiguration
{
    public static void UseSwaggerUI(this IApplicationBuilder app) => app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Core Divdados");
        options.RoutePrefix = string.Empty;
    });

    public static void AddSwagger(this IServiceCollection services) => services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Core Divdados",
            Version = "v1",
            Description = "Core do projeto de TG da equipe DivDados",
            Contact = new OpenApiContact
            {
                Name = "Danilo Rodrigues",
            }
        });
        options.CustomSchemaIds(option => option.FullName);

        var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;
        var applicationName = PlatformServices.Default.Application.ApplicationName;
        var xmlDocumentPath = Path.Combine(applicationBasePath, $"{applicationName}.xml");
        if (File.Exists(xmlDocumentPath)) options.IncludeXmlComments(xmlDocumentPath);
    });
}
