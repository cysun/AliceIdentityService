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
        private void CheckDefaultIdentityResources()
        {
            var defaultResourceList = new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
                new IdentityResources.Address()
            };
            var defaultResources = defaultResourceList.ToDictionary(r => r.Name, r => r);

            var resources = configDbContext.IdentityResources
                .Where(r => defaultResources.Keys.Contains(r.Name))
                .ToDictionary(r => r.Name, r => r);
            foreach (var defaultResource in defaultResources)
            {
                if (!resources.ContainsKey(defaultResource.Key))
                    configDbContext.IdentityResources.Add(defaultResource.Value.ToEntity());
            }
            configDbContext.SaveChanges();
        }
    }
}
