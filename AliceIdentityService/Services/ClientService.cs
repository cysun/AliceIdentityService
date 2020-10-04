using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AliceIdentityService.Services
{
    public class ClientService
    {
        private readonly AppConfigurationDbContext _db;

        public ClientService(AppConfigurationDbContext db)
        {
            _db = db;
        }

        public List<Client> GetClients()
        {
            return _db.Clients.Include(c => c.Properties).ToList();
        }

        public Client GetClient(int id)
        {
            return _db.Clients.Where(c => c.Id == id)
                .Include(c => c.Properties)
                .Include(c => c.ClientSecrets)
                .Include(c => c.RedirectUris)
                .Include(c => c.AllowedGrantTypes)
                .Include(c => c.AllowedScopes)
                .SingleOrDefault();
        }

        public void AddClient(Client client)
        {
            _db.Clients.Add(client);
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
