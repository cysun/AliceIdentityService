using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;

namespace ConsoleManager
{
    partial class ConsoleManager
    {
        private void ClientsController()
        {
            var done = false;
            do
            {
                var clients = configDbContext.Clients.Select(e => e.ToModel()).ToList();
                var cmd = ClientsView(clients);
                switch (cmd)
                {
                    case "a":
                        break;
                    case "b":
                        done = true;
                        break;
                    default:
                        int index;
                        bool isNumber = int.TryParse(cmd, out index);
                        if (isNumber && index < clients.Count)
                            ClientView(clients[index]);
                        break;
                }
            } while (!done);
        }

        private string ClientsView(List<Client> clients)
        {
            var validChoices = new HashSet<string>() { "a", "b" };
            for (int i = 0; i < clients.Count; ++i)
                validChoices.Add(i.ToString());

            string choice;
            do
            {
                Console.Clear();
                Console.WriteLine("\t Client Management \n");
                Console.WriteLine("\t a) Add a client");
                Console.WriteLine("\t b) Back to Main Menu\n");
                for (int i = 0; i < clients.Count; ++i)
                    Console.WriteLine($"\t {i}) {clients[i].ClientId}");
                Console.Write("\n Pleasse enter your choice: ");
                choice = Console.ReadLine().ToLower();
            } while (!validChoices.Contains(choice));
            return choice;
        }

        private void ClientView(Client client)
        {
            Console.Clear();
            Console.WriteLine($"\t Client Management - {client.ClientId} \n");
            Console.Write("\n Press [Enter] key to go back");
            Console.ReadLine();
        }
    }
}
