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
    public class ApiScopesController : Controller
    {
        private readonly ApiScopeService _apiScopeService;

        private readonly IMapper _mapper;
        private readonly ILogger<ApiScopesController> _logger;

        public ApiScopesController(ApiScopeService apiScopeService, IMapper mapper, ILogger<ApiScopesController> logger)
        {
            _apiScopeService = apiScopeService;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_apiScopeService.GetApiScopes());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new ApiScopeInputModel());
        }

        [HttpPost]
        public IActionResult Add(ApiScopeInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var apiScope = _mapper.Map<ApiScope>(input);
            _apiScopeService.AddApiScope(apiScope);
            _apiScopeService.SaveChanges();

            _logger.LogInformation("{user} created apiScope {apiScope}", User.Identity.Name, apiScope.Name);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var apiScope = _apiScopeService.GetApiScope(id);
            if (apiScope == null) return NotFound();

            return View(_mapper.Map<ApiScopeInputModel>(apiScope));
        }

        [HttpPost]
        public IActionResult Edit(int id, ApiScopeInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var apiScope = _apiScopeService.GetApiScope(id);
            _mapper.Map(input, apiScope);
            _apiScopeService.SaveChanges();

            _logger.LogInformation("{user} edited ApiScope {apiScope}", User.Identity.Name, id);

            return RedirectToAction("Index");
        }
    }
}

namespace AliceIdentityService.Models
{
    public class ApiScopeInputModel
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; } = true;
    }
}
