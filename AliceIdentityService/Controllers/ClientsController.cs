using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AliceIdentityService.Models;
using AliceIdentityService.Security;
using AliceIdentityService.Services;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AliceIdentityService.Controllers
{
    [Authorize(Policy = Policy.IsAdministrator)]
    public class ClientsController : Controller
    {
        private readonly ClientService _clientService;

        public ClientsController(ClientService clientService)
        {
            _clientService = clientService;
        }

        public IActionResult Index()
        {
            return View(_clientService.GetClients());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new ClientInputModel());
        }

        [HttpPost]
        public IActionResult Add(ClientInputModel input)
        {
            var client = new Client
            {
                ClientId = input.Id,
                ClientName = input.Name,
                ClientSecrets = { new Secret(input.Secret.Sha256()) },
                RedirectUris = { input.RedirectUrl },
                AllowOfflineAccess = true,
                AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email
                    },
                AllowedGrantTypes = GrantTypes.Code
            };
            _clientService.AddClient(client);
            _clientService.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

namespace AliceIdentityService.Models
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
