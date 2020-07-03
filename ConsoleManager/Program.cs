using System;
using System.Collections.Generic;
using System.IO;
using AliceIdentityService.Models;
using AliceIdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ConsoleManager
{
    partial class ConsoleManager
    {
        private readonly ServiceProvider serviceProvider;

        public UserManager<ApplicationUser> UserManager => serviceProvider.GetService<UserManager<ApplicationUser>>();

        public ConsoleManager()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AliceIdentityService"))
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<AppDbContext>();
            serviceProvider = services.BuildServiceProvider();

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            foreach (var user in userManager.Users)
                Console.WriteLine(user.UserName);
        }

        public void MainController()
        {
            var done = false;
            do
            {
                var cmd = MainView();
                switch (cmd)
                {
                    case "u":
                        UsersController();
                        break;
                    case "x":
                        done = true;
                        break;
                }
            } while (!done);
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
        static void Main(string[] args)
        {
            (new ConsoleManager()).MainController();
        }
    }
}
