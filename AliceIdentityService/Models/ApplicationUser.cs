using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AliceIdentityService.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData, MaxLength(255)]
        public string FirstName { get; set; }

        [PersonalData, MaxLength(255)]
        public string LastName { get; set; }

        [MaxLength(50)]
        public string Nickname { get; set; }
    }
}
