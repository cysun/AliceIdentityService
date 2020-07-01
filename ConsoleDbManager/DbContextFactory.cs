using System.IO;
using AliceIdentityService.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleDbManager
{
    class DbContextFactory
    {
        static readonly DbContextOptions<IdentityDbContext<ApplicationUser>> IdentityDbOptions;
        static readonly DbContextOptions<ConfigurationDbContext> ConfigurationDbOptions;

        static DbContextFactory()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AliceIdentityService"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder1 = new DbContextOptionsBuilder<IdentityDbContext<ApplicationUser>>();
            optionsBuilder1.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            IdentityDbOptions = optionsBuilder1.Options;

            var optionsBuilder2 = new DbContextOptionsBuilder<ConfigurationDbContext>();
            optionsBuilder2.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            ConfigurationDbOptions = optionsBuilder2.Options;
        }

        public static IdentityDbContext<ApplicationUser> GetIdentityDbContext()
        {
            return new IdentityDbContext<ApplicationUser>(IdentityDbOptions);
        }

        public static ConfigurationDbContext<ConfigurationDbContext> GetConfigurationDbContext()
        {
            return new ConfigurationDbContext(ConfigurationDbOptions, new ConfigurationStoreOptions());
        }
    }
}
