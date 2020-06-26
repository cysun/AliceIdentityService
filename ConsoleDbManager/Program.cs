using System;
using System.Linq;

namespace ConsoleDbManager
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new AppDbContext();
            var users = db.Users.Select(u => u);
            foreach (var user in users)
                Console.WriteLine(user);
        }
    }
}
