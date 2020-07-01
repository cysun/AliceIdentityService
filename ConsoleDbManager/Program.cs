using System;
using System.Linq;

namespace ConsoleDbManager
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = DbContextFactory.GetIdentityDbContext();
            var users = db.Users.Select(u => u);
            foreach (var user in users)
                Console.WriteLine(user);

            using var db2 = DbContextFactory.GetConfigurationDbContext();
            var resources = db2.ApiResources.Select(r => r);
            foreach (var resource in resources)
                Console.WriteLine(resource.Name);
        }
    }
}
