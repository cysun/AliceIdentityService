using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AliceIdentityService.Models;

namespace ConsoleManager
{
    partial class ConsoleManager
    {
        public void UsersController()
        {
            var done = false;
            do
            {
                var users = UserManager.Users.OrderBy(u => u.UserName).ToList();
                var cmd = UsersView(users);
                if (cmd == "m")
                {
                    done = true;
                }
                else
                {
                    int index;
                    bool isNumber = int.TryParse(cmd, out index);
                    if (isNumber && index < users.Count)
                        UserView(users[index]);
                }
            } while (!done);
        }

        public string UsersView(List<ApplicationUser> users)
        {
            var validChoices = new HashSet<string>() { "m" };
            for (int i = 0; i < users.Count; ++i)
                validChoices.Add(i.ToString());

            string choice;
            do
            {
                Console.Clear();
                Console.WriteLine($"\t User Management \n");
                Console.WriteLine("\t m) Back to Main Menu");
                for (int i = 0; i < users.Count; ++i)
                    Console.WriteLine($"\t {i}) {users[i].UserName}");
                Console.Write("\n Pleasse enter your choice: ");
                choice = Console.ReadLine().ToLower();
            } while (!validChoices.Contains(choice));
            return choice;
        }

        public void UserView(ApplicationUser user)
        {
            Console.Clear();
            Console.WriteLine($"\t User Management - {user.UserName} \n");
            Console.Write("\n Press [Enter] key to go back");
            Console.ReadLine().ToLower();
        }
    }
}
