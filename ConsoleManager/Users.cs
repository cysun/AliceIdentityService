using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AliceIdentityService.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ConsoleManager
{
    partial class ConsoleManager
    {
        private async Task UsersControllerAsync()
        {
            var done = false;
            do
            {
                var users = userManager.Users.OrderBy(u => u.UserName).ToList();
                var cmd = UsersView(users);
                switch (cmd)
                {
                    case "a":
                        await AddUserAsync();
                        break;
                    case "b":
                        done = true;
                        break;
                    default:
                        int index;
                        bool isNumber = int.TryParse(cmd, out index);
                        if (isNumber && index < users.Count)
                            UserView(users[index]);
                        break;
                }
            } while (!done);
        }

        private string UsersView(List<ApplicationUser> users)
        {
            var validChoices = new HashSet<string>() { "a", "b" };
            for (int i = 0; i < users.Count; ++i)
                validChoices.Add(i.ToString());

            string choice;
            do
            {
                Console.Clear();
                Console.WriteLine("\t User Management \n");
                Console.WriteLine("\t a) Add a user");
                Console.WriteLine("\t b) Back to Main Menu\n");
                for (int i = 0; i < users.Count; ++i)
                    Console.WriteLine($"\t {i}) {users[i].UserName}");
                Console.Write("\n Pleasse enter your choice: ");
                choice = Console.ReadLine().ToLower();
            } while (!validChoices.Contains(choice));
            return choice;
        }

        private void UserView(ApplicationUser user)
        {
            Console.Clear();
            Console.WriteLine($"\t User Management - {user.UserName} \n");
            Console.Write("\n Press [Enter] key to go back");
            Console.ReadLine();
        }

        private async Task AddUserAsync()
        {
            Console.Clear();
            Console.WriteLine("\t Add User \n");
            Console.Write("\t Username: ");
            var username = Console.ReadLine();
            Console.Write("\t Password: ");
            var password = Console.ReadLine();
            Console.Write("\t email: ");
            var email = Console.ReadLine();
            Console.Write("\t Administrator? [y|n]: ");
            bool isAdmin = Console.ReadLine().ToLower() == "y";
            Console.Write("\t Save or Cancel? [s|c] ");
            var cmd = Console.ReadLine();
            if (cmd.ToLower() == "s")
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    await userManager.ConfirmEmailAsync(user, token);
                    if (isAdmin)
                        await userManager.AddToRoleAsync(user, AdminRoleName);
                }
                else
                {
                    Console.WriteLine("\n\t Failed to create the user");
                    foreach (var error in result.Errors)
                        Console.WriteLine($"\t {error.Description}");
                    Console.Write("\n\n\t Press [Enter] key to continue");
                    Console.ReadLine();
                }
            }
        }
    }
}
