using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace AliceIdentityService.Services
{
    public class IdentityResourceService
    {
        private readonly AppConfigurationDbContext _db;

        public IdentityResourceService(AppConfigurationDbContext db)
        {
            _db = db;
        }

        public List<IdentityResource> GetIdentityResources()
        {
            return _db.IdentityResources.ToList();
        }

        public IdentityResource GetIdentityResource(int id)
        {
            return _db.IdentityResources.Where(r => r.Id == id).Include(r => r.UserClaims).FirstOrDefault();
        }

        public IdentityResourceClaim GetIdentityResourceClaim(int id)
        {
            return _db.identityResourceClaims.Where(c => c.Id == id).Include(c => c.IdentityResource).SingleOrDefault();
        }

        public void DeleteIdentityResourceClaim(int id)
        {
            _db.identityResourceClaims.Remove(new IdentityResourceClaim { Id = id });
        }

        public void AddIdentityResource(IdentityResource identityResource)
        {
            _db.IdentityResources.Add(identityResource);
        }

        public void SaveChanges() => _db.SaveChanges();
    }
}
