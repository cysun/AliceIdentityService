using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AliceIdentityService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleDbManager
{
    class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private static readonly string ConnectionString;

        static AppDbContext()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AliceIdentityService"))
                .AddJsonFile("appsettings.json")
                .Build();
            ConnectionString = config.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConnectionString);
        }
    }
}
