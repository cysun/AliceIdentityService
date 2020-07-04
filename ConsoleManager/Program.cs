using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AliceIdentityService.Models;
using AliceIdentityService.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Storage;
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

        ConfigurationDbContext<ConfigurationDbContext> configDbContext => serviceProvider.GetService<ConfigurationDbContext>();

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
            services.AddConfigurationDbContext(options =>
                options.ConfigureDbContext = db => db.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            serviceProvider = services.BuildServiceProvider();
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
                    case "c":
                        ClientsController();
                        break;
                    case "x":
                        done = true;
                        break;
                }
            } while (!done);

            serviceProvider.Dispose();
        }

        public string MainView()
        {
            var validChoices = new HashSet<string>() { "u", "c", "x" };
            string choice;
            do
            {
                Console.Clear();
                Console.WriteLine("\t Main Menu \n");
                Console.WriteLine("\t u) User Management");
                Console.WriteLine("\t c) Client Management");
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
