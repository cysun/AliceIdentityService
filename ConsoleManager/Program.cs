using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AliceIdentityService.Models;
using AliceIdentityService.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ConsoleManager
{
    partial class ConsoleManager
    {
        const string AdminRoleName = "Administrator";

        readonly ServiceProvider serviceProvider;

        UserManager<ApplicationUser> userManager => serviceProvider.GetService<UserManager<ApplicationUser>>();

        RoleManager<IdentityRole> roleManager => serviceProvider.GetService<RoleManager<IdentityRole>>();

        ConfigurationDbContext<ConfigurationDbContext> configDbContext;

        public ConsoleManager()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AliceIdentityService"))
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
            serviceProvider = services.BuildServiceProvider();

            var optionsBuilder2 = new DbContextOptionsBuilder<ConfigurationDbContext>();
            optionsBuilder2.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            var ConfigurationDbOptions = optionsBuilder2.Options;
            configDbContext = new ConfigurationDbContext(ConfigurationDbOptions, new ConfigurationStoreOptions());
        }

        public async Task MainControllerAsync()
        {
            await CheckAdminRoleAsync();
            CheckDefaultIdentityResources();

            var done = false;
            do
            {
                var cmd = MainView();
                switch (cmd)
                {
                    case "u":
                        await UsersControllerAsync();
                        break;
                    case "x":
                        done = true;
                        break;
                }
            } while (!done);

            serviceProvider.Dispose();
            configDbContext.Dispose();
        }

        public string MainView()
        {
            var validChoices = new HashSet<string>() { "u", "x" };
            string choice;
            do
            {
                Console.Clear();
                Console.WriteLine("\t Main Menu \n");
                Console.WriteLine("\t u) User Management");
                Console.WriteLine("\t x) Exit");
                Console.Write("\n  Pleasse enter your choice: ");
                choice = Console.ReadLine().ToLower();
            } while (!validChoices.Contains(choice));

            return choice;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            await (new ConsoleManager()).MainControllerAsync();
        }
    }
}
