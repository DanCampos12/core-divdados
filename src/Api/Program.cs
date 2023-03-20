using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using System.IO;
using Microsoft.Extensions.Configuration;
using System;

namespace Core.Divdados.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var host = BuildWebHost(args);
        host.Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                var appsettingsName = $"appsettings.{env.EnvironmentName}.json";
                config.AddJsonFile(appsettingsName, optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
                Console.WriteLine($"Appsettings: {appsettingsName}");
            })
            .UseStartup<Startup>()
            .Build();
}
