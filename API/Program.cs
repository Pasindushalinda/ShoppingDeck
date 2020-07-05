using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        [Obsolete]
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var loggerFactory = service.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = service.GetRequiredService<StoreContext>();
                    await context.Database.MigrateAsync();

                    // context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT ProductBrands ON");
                    // context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT ProductTypes ON");

                    await StoreContextSeed.SeedAsync(context, loggerFactory);

                    // context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT ProductBrands OFF");
                    // context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT ProductTypes OFF");
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "Error occured during migration");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
