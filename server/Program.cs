using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orchestrate.API.Data;
using System;

namespace Orchestrate.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            InitDb(host);

            host.Run();
        }

        private static void InitDb(IHost host)
        {
            using var scope = host.Services.CreateScope();
            try
            {
                scope.ServiceProvider.GetRequiredService<OrchestrateDbInitializer>().Initialize();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                logger.LogError(ex, "An error occured while initializing the DB");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
