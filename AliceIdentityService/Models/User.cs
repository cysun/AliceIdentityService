using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using AliceIdentityService.Security;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace AliceIdentityService.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(255)]
        [PersonalData]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        [PersonalData]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nickname { get; set; }

        public bool IsAdministrator { get; set; }

        public string Name => $"{FirstName} {LastName}";

        public List<Claim> Claims() => new List<Claim>
        {
            new Claim(JwtClaimTypes.GivenName, FirstName),
            new Claim(JwtClaimTypes.FamilyName, LastName),
            new Claim(JwtClaimTypes.NickName, Nickname),
            new Claim(JwtClaimTypes.Name, Name),
            new Claim(JwtClaimTypes.Email, Email),
            new Claim(AliceClaimTypes.IsAdministrator, IsAdministrator.ToString())
        };
    }
}
