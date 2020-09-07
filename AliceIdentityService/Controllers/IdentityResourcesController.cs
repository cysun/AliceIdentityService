using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AliceIdentityService.Models;
using AliceIdentityService.Security;
using AliceIdentityService.Services;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AliceIdentityService.Controllers
{
    [Authorize(Policy = Policy.IsAdministrator)]
    public class IdentityResourcesController : Controller
    {
        private readonly IdentityResourceService _identityResourceService;

        private readonly IMapper _mapper;
        private readonly ILogger<IdentityResourcesController> _logger;

        public IdentityResourcesController(IdentityResourceService identityResourceService,
            IMapper mapper, ILogger<IdentityResourcesController> logger)
        {
            _identityResourceService = identityResourceService;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_identityResourceService.GetIdentityResources());
        }

        public IActionResult View(int id)
        {
            var resource = _identityResourceService.GetIdentityResource(id);
            if (resource == null) return NotFound();

            return View(resource);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new IdentityResourceInputModel());
        }

        public IActionResult Add(IdentityResourceInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var identityResource = _mapper.Map<IdentityResource>(input);
            identityResource.Created = DateTime.Now;
            _identityResourceService.AddIdentityResource(identityResource);
            _identityResourceService.SaveChanges();

            _logger.LogInformation("{user} created identity resource {identityResource}",
                User.Identity.Name, identityResource);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var identityResource = _identityResourceService.GetIdentityResource(id);
            if (identityResource == null) return NotFound();

            return View(_mapper.Map<IdentityResourceInputModel>(identityResource));
        }

        [HttpPost]
        public IActionResult Edit(int id, IdentityResourceInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var identityResource = _identityResourceService.GetIdentityResource(id);
            _mapper.Map(input, identityResource);
            identityResource.Updated = DateTime.Now;
            _identityResourceService.SaveChanges();

            _logger.LogInformation("{user} edited identity resource {identityResource}", User.Identity.Name, id);

            return RedirectToAction("Index");
        }

        [HttpGet("IdentityResources/{resourceId}/AddClaim")]
        public IActionResult AddClaim(int resourceId)
        {
            ViewBag.Resource = _identityResourceService.GetIdentityResource(resourceId);
            return View(new IdentityResourceClaimInputModel());
        }

        [HttpPost("IdentityResources/{resourceId}/AddClaim")]
        public IActionResult AddClaim(int resourceId, IdentityResourceClaimInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var claim = _mapper.Map<IdentityResourceClaim>(input);
            var resource = _identityResourceService.GetIdentityResource(resourceId);
            resource.UserClaims.Add(claim);
            _identityResourceService.SaveChanges();

            _logger.LogInformation("{user} added claim {userClaim} to identity resource {identityResource}",
                User.Identity.Name, claim.Id, resourceId);

            return RedirectToAction("View", new { id = resourceId });
        }

        [HttpGet("IdentityResources/{resourceId}/EditClaim/{claimId}")]
        public IActionResult EditClaim(int claimId)
        {
            var claim = _identityResourceService.GetIdentityResourceClaim(claimId);
            if (claim == null) return NotFound();

            ViewBag.ClaimId = claimId;
            ViewBag.Resource = claim.IdentityResource;
            return View(_mapper.Map<IdentityResourceClaimInputModel>(claim));
        }

        [HttpPost("IdentityResources/{resourceId}/EditClaim/{claimId}")]
        public IActionResult EditClaim(int claimId, IdentityResourceClaimInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var claim = _identityResourceService.GetIdentityResourceClaim(claimId);
            _mapper.Map(input, claim);
            _identityResourceService.SaveChanges();

            _logger.LogInformation("{user} edited claim {userClaim} in identity resouce {identityResource}",
                User.Identity.Name, claim.Id, claim.IdentityResourceId);

            return RedirectToAction("View", new { id = claim.IdentityResourceId });
        }

        [Route("IdentityResources/{resourceId}/DeleteClaim/{claimId}")]
        public IActionResult DeleteClaim(int resourceId, int claimId)
        {
            _identityResourceService.DeleteIdentityResourceClaim(claimId);
            _identityResourceService.SaveChanges();

            _logger.LogInformation("{user} deleted claim {userClaim} from identity resurce {identityResource}",
                User.Identity.Name, claimId, resourceId);

            return RedirectToAction("View", new { id = resourceId });
        }
    }
}

namespace AliceIdentityService.Models
{
    public class IdentityResourceInputModel
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(200)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Display(Name = "Required (i.e. cannot de-select) on content screen")]
        public bool Required { get; set; } = false;

        [Display(Name = "Emphasized on content screen")]
        public bool Emphasize { get; set; } = false;

        [Display(Name = "Show in Discovery Document")]
        public bool ShowInDiscoveryDocument { get; set; } = true;

        public bool Enabled { get; set; } = true;
    }

    public class IdentityResourceClaimInputModel
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "Claim Type")]
        public string Type { get; set; }
    }
}
