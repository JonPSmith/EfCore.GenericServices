using System;
using System.IO;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceLayer.DatabaseCode.Services;

namespace RazorPageApp.Helpers
{
    public static class DatabaseStartupHelpers
    {

        private static readonly string WwwRootDirectory = $"wwwroot{Path.DirectorySeparatorChar}";

        public static string GetWwwRootPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), WwwRootDirectory);
        }

        public static IWebHost SetupDevelopmentDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<EfCoreContext>())
                {
                    try
                    {
                        context.DevelopmentEnsureCreated();
                        context.SeedDatabase(GetWwwRootPath());
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while setting up or seeding the development database.");
                    }
                }
            }

            return webHost;
        }
    }
}