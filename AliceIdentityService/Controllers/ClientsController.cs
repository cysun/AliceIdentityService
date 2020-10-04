using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AliceIdentityService.Models;
using AliceIdentityService.Security;
using AliceIdentityService.Services;
using AutoMapper;
using IdentityServer4;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AliceIdentityService.Controllers
{
    [Authorize(Policy = Policy.IsAdministrator)]
    public class ClientsController : Controller
    {
        private readonly ClientService _clientService;
        private readonly IdentityResourceService _identityResourceService;
        private readonly ApiScopeService _apiScopeService;

        private readonly IMapper _mapper;
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(ClientService clientService, IdentityResourceService identityResourceService,
            ApiScopeService apiScopeService, IMapper mapper, ILogger<ClientsController> logger)
        {
            _clientService = clientService;
            _identityResourceService = identityResourceService;
            _apiScopeService = apiScopeService;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_clientService.GetClients());
        }

        [HttpGet]
        public IActionResult View(int id)
        {
            var client = _clientService.GetClient(id);
            if (client == null) return NotFound();

            ViewBag.IdentityResources = _identityResourceService.GetIdentityResources();
            ViewBag.ApiScopes = _apiScopeService.GetApiScopes();
            ViewBag.AllowedScopes = client.AllowedScopes.Select(s => s.Scope).ToList();

            var input = _mapper.Map<ClientInputModel>(client);
            input.ClientType = client.GetClientType();
            return View(input);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new ClientInputModel());
        }

        [HttpPost]
        public IActionResult Add(ClientInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var client = _mapper.Map<Client>(input);
            client.SetClientType(input.ClientType);
            _clientService.AddClient(client);
            _clientService.SaveChanges();

            _logger.LogInformation("{user} created client {client}", User.Identity.Name, client.ClientId);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var client = _clientService.GetClient(id);
            if (client == null) return NotFound();

            var input = _mapper.Map<ClientInputModel>(client);
            input.ClientType = client.GetClientType();
            return View(input);
        }

        [HttpPost]
        public IActionResult Edit(int id, ClientInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var client = _clientService.GetClient(id);
            _mapper.Map(input, client);
            client.SetClientType(input.ClientType);
            _clientService.SaveChanges();

            _logger.LogInformation("{user} edited client {client}", User.Identity.Name, id);

            return RedirectToAction("Index");
        }

        [HttpPut("Clients/{id}/AllowedScopes/{scope}")]
        public IActionResult AddAllowedScope(int id, string scope)
        {
            var client = _clientService.GetClient(id);
            if (client == null) return NotFound();

            var allowedScope = client.AllowedScopes.Where(s => s.Scope == scope).SingleOrDefault();
            if (allowedScope == null)
            {
                client.AllowedScopes.Add(new ClientScope
                {
                    Scope = scope
                });
                _clientService.SaveChanges();
                _logger.LogInformation("{user} added {scope} to client {client}", User.Identity.Name, scope, id);
            }

            return Ok();
        }

        [HttpDelete("Clients/{id}/AllowedScopes/{scope}")]
        public IActionResult RemoveAllowedScope(int id, string scope)
        {
            var client = _clientService.GetClient(id);
            if (client == null) return NotFound();

            int removed = client.AllowedScopes.RemoveAll(s => s.Scope == scope);
            if (removed > 0)
            {
                _clientService.SaveChanges();
                _logger.LogInformation("{user} removed {scope} to client {client}", User.Identity.Name, scope, id);
            }

            return Ok();
        }
    }
}

namespace AliceIdentityService.Models
{
    public enum ClientType
    {
        Native, // mobile, desktop, CLI, and smart device apps running natively
        SPA, // Single-Page Application - a JavaScript frontend app that uses an API
        WebApp, // Traditional web application using redirects
        Noninteractive // CLIs, deamons or services running in backend
    }

    public class ClientInputModel
    {
        public int Id { get; set; }

        public bool Enabled { get; set; } = true;

        [Required]
        [MaxLength(200)]
        [Display(Name = "Client Id")]
        public string ClientId { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Protocol Type")]
        public string ProtocolType { get; set; } = "oidc";

        // List<ClientSecret> <=> string
        [Display(Name = "Client Secret")]
        public string ClientSecrets { get; set; }

        [Display(Name = "Require Client Secret")]
        public bool RequireClientSecret { get; set; }

        [MaxLength(200)]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Client URI")]
        public string ClientUri { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Logo URI")]
        public string LogoUri { get; set; }

        [Display(Name = "Require Content")]
        public bool RequireConsent { get; set; } = false;

        [Display(Name = "Allow Remember Content")]
        public bool AllowRememberConsent { get; set; } = true;

        [Display(Name = "Always Include User Claims in Id Token")]
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        // List<ClientGrantType> <=> string
        [Display(Name = "Allowed Grant Types")]
        public string AllowedGrantTypes { get; set; } = IdentityServer4.Models.GrantType.AuthorizationCode;

        [Display(Name = "Required PKCE")]
        public bool RequirePkce { get; set; } = true;

        [Display(Name = "Allow Plain Text PKCE")]
        public bool AllowPlainTextPkce { get; set; }

        // public bool RequireRequestObject { get; set; }
        // public bool AllowAccessTokensViaBrowser { get; set; }

        // List<ClientRedirectUri> <=> string, currently only allow one
        [Display(Name = "Redirect URI")]
        public string RedirectUris { get; set; }

        // public List<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; }
        // public string FrontChannelLogoutUri { get; set; }
        // public bool FrontChannelLogoutSessionRequired { get; set; } = true;
        // public string BackChannelLogoutUri { get; set; }
        // public bool BackChannelLogoutSessionRequired { get; set; } = true;
        // public bool AllowOfflineAccess { get; set; }

        // public List<ClientScope> AllowedScopes { get; set; }

        [Display(Name = "Identity Token Lifetime")]
        public int IdentityTokenLifetime { get; set; } = 300;

        // public string AllowedIdentityTokenSigningAlgorithms { get; set; }

        [Display(Name = "Access Token Lifetime")]
        public int AccessTokenLifetime { get; set; } = 3600;

        [Display(Name = "Authorization Code Lifetime")]
        public int AuthorizationCodeLifetime { get; set; } = 300;

        // public int? ConsentLifetime { get; set; } = null;
        // public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        // public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        // public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;
        // public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        // public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;
        // public int AccessTokenType { get; set; } = (int)0; // AccessTokenType.Jwt;
        // public bool EnableLocalLogin { get; set; } = true;
        // public List<ClientIdPRestriction> IdentityProviderRestrictions { get; set; }
        // public bool IncludeJwtId { get; set; }
        // public List<ClientClaim> Claims { get; set; }
        // public bool AlwaysSendClientClaims { get; set; }
        // public string ClientClaimsPrefix { get; set; } = "client_";
        // public string PairWiseSubjectSalt { get; set; }

        // List<ClientCorsOrigin> <=> string, currently only allow one
        [Display(Name = "Client CORS Origin")]
        public string AllowedCorsOrigins { get; set; }

        // public List<ClientProperty> Properties { get; set; }

        // ClientType is stored as a ClientProperty
        [Required]
        [Display(Name = "Client Type")]
        public ClientType ClientType { get; set; } = ClientType.WebApp;

        // public DateTime Created { get; set; } = DateTime.UtcNow;
        // public DateTime? Updated { get; set; }
        // public DateTime? LastAccessed { get; set; }
        // public int? UserSsoLifetime { get; set; }
        // public string UserCodeType { get; set; }
        // public int DeviceCodeLifetime { get; set; } = 300;
        // public bool NonEditable { get; set; }
    }
}
