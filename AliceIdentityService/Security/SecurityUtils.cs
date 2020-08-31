using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AliceIdentityService.Models;

namespace AliceIdentityService.Security
{
    public class SecurityUtils
    {
        public static List<Claim> GetAdditionalClaims(User user)
        {
            var claims = new List<Claim>();

            if (user.IsAdministrator)
                claims.Add(new Claim(ClaimType.IsAdministrator, "true"));

            return claims;
        }
    }
}
