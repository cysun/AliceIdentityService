using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AliceIdentityService.ViewModels
{
    public class ClientInputModel
    {
        [Required]
        [Display(Name = "Client Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Client ID")]
        public string Id { get; set; }

        [Display(Name = "Client Secret")]
        public string Secret { get; set; }

        [Display(Name = "Redirect URL")]
        public string RedirectUrl { get; set; }
    }
}
