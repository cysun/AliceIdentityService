using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IdentityServer4;
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
                        AddClient();
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


        private void AddClient()
        {
            Console.Clear();
            Console.WriteLine("\t Add Client \n");
            Console.Write("\t Client Id: ");
            var id = Console.ReadLine();
            Console.Write("\t Client Name: ");
            var name = Console.ReadLine();
            var secret = Guid.NewGuid().ToString();
            Console.Write($"\t Client Secret: {secret} \n");

            var validTypes = new HashSet<string> { "mvc" };
            string type;
            do
            {
                Console.Write("\t Client Type [mvc]: ");
                type = Console.ReadLine().ToLower();
            } while (!validTypes.Contains(type));

            Console.Write("\t Redirect URL: ");
            var redirectUrl = Console.ReadLine();

            Console.Write("\t Save or Cancel? [s|c] ");
            var cmd = Console.ReadLine();
            if (cmd.ToLower() == "s")
            {
                var client = new Client
                {
                    ClientId = id,
                    ClientName = name,
                    ClientSecrets = { new Secret(secret.Sha256()) },
                    RedirectUris = { redirectUrl },
                    AllowOfflineAccess = true,
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                };

                switch (type)
                {
                    case "mvc":
                        client.AllowedGrantTypes = GrantTypes.Code;
                        break;
                    default:
                        Console.WriteLine($"Unsupported client type: {type}");
                        break;
                }

                configDbContext.Clients.Add(client.ToEntity());
                configDbContext.SaveChanges();
            }
        }
    }
}
