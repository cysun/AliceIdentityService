using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;

namespace AliceIdentityService.Services
{
    public class ClientService
    {
        private readonly ConfigurationDbContext _db;

        public ClientService(ConfigurationDbContext db)
        {
            _db = db;
        }

        public List<Client> GetClients()
        {
            return _db.Clients.Select(c => c.ToModel()).ToList();
        }

        public void AddClient(Client client)
        {
            _db.Clients.Add(client.ToEntity());
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
