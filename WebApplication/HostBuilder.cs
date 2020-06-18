using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace WebApplication
{
    public class HostBuilder
    {
        public static IHostBuilder Foundation<TStartUp>(string[] args) where TStartUp : class
        {
            
            return Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, configBuilder) =>
                 {
                     var builtConfig = configBuilder.Build();

                     if (builtConfig["appsettings"] != null)
                     {
                         Console.WriteLine("appsettings found in environment, importing config");

                         var memoryFileProvider = new InMemoryFileProvider(builtConfig["appsettings"]);
                         var configuration = new ConfigurationBuilder()
                             .AddJsonFile(memoryFileProvider, "env_appsettings.json", false, false)
                             .Build();
                     }
                 })
                 .ConfigureLogging((loggingBuilder) =>
                 {
                     loggingBuilder.AddConsole();
                 })
                 .ConfigureServices((services) =>
                 {
                     services.AddHttpClient();
                     services.AddHealthChecks().AddCheck("Health Check", () => HealthCheckResult.Healthy($"Application is running", GetData() ), tags: new[] { "all" });


                 })
                 .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseStartup<TStartUp>();
                 });
        }

        private static IReadOnlyDictionary<string, object> GetData()
        {
            return new Dictionary<string, object>() {
                { "Environment.MachineName" , Environment.MachineName.ToString() },
                { "Environment.OSVersion" , Environment.OSVersion.ToString() },
                { "Environment.ProcessorCount" , Environment.ProcessorCount.ToString() },
                { "Environment.WorkingSet" , Environment.WorkingSet.ToString() },
            };
        }
    }
}
