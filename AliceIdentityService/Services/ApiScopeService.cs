using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;

namespace AliceIdentityService.Services
{
    public class ApiScopeService
    {
        private readonly AppConfigurationDbContext _db;

        public ApiScopeService(AppConfigurationDbContext db)
        {
            _db = db;
        }

        public List<ApiScope> GetApiScopes()
        {
            return _db.ApiScopes.ToList();
        }

        public ApiScope GetApiScope(int id)
        {
            return _db.ApiScopes.Find(id);
        }

        public void AddApiScope(ApiScope apiScope)
        {
            _db.ApiScopes.Add(apiScope);
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
