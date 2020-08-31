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
        readonly ServiceProvider serviceProvider;

        UserManager<User> userManager => serviceProvider.GetService<UserManager<User>>();

        ConfigurationDbContext<ConfigurationDbContext> configDbContext => serviceProvider.GetService<ConfigurationDbContext>();

        public ConsoleManager()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AliceIdentityService"))
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();
            services.AddOptions().AddLogging();
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedEmail = true)
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddConfigurationDbContext(options =>
                options.ConfigureDbContext = db => db.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            serviceProvider = services.BuildServiceProvider();
        }

        public async Task MainControllerAsync()
        {
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
