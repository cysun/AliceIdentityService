﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AliceIdentityService.Models;
using AliceIdentityService.Security;
using AliceIdentityService.Services;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AliceIdentityService.Controllers
{
    [Authorize(Policy = Policy.IsAdministrator)]
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;

        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserService userService, UserManager<User> userManager,
            IMapper mapper, ILogger<UsersController> logger)
        {
            _userService = userService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(_userService.GetUsers());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new NewUserInputModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(NewUserInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var user = _mapper.Map<User>(input);
            user.UserName = input.Email;
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                var claims = SecurityUtils.GetAdditionalClaims(user);
                if (claims.Count > 0)
                    await _userManager.AddClaimsAsync(user, claims);

                _logger.LogInformation("{user} created account for {newUser}", User.Identity.Name, input.Email);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(input);
            }
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var user = _userService.GetUser(id);
            if (user == null) return NotFound();

            return View(_mapper.Map<EditUserInputModel>(user));
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(string id, EditUserInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var user = _userService.GetUser(id);
            var oldClaims = SecurityUtils.GetAdditionalClaims(user);

            if (!string.IsNullOrWhiteSpace(input.Password))
            {
                var result = await ChangePaswordAsync(user, input.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("Password", error.Description);
                    return View(input);
                }
            }

            _mapper.Map<EditUserInputModel, User>(input, user);
            _userService.SaveChanges();

            var newClaims = SecurityUtils.GetAdditionalClaims(user);
            if (oldClaims.Count > 0)
                await _userManager.RemoveClaimsAsync(user, oldClaims);
            if (newClaims.Count > 0)
                await _userManager.AddClaimsAsync(user, newClaims);

            _logger.LogInformation("{user} edited account {account}", User.Identity.Name, input.Email);

            return RedirectToAction(nameof(Index));
        }

        private async Task<IdentityResult> ChangePaswordAsync(User user, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
    }
}

namespace AliceIdentityService.Models
{
    public class NewUserInputModel : RegistrationInputModel
    {
        [Display(Name = "Administrator")]
        public bool IsAdministrator { get; set; }

        [Display(Name = "Event Organizer")]
        public bool IsEventOrganizer { get; set; }

        [Display(Name = "Event Reviewer")]
        public bool IsEventReviewer { get; set; }

        [Display(Name = "Reward Provider")]
        public bool IsRewardProvider { get; set; }

        [Display(Name = "Reward Reviewer")]
        public bool IsRewardReviewer { get; set; }
    }

    public class EditUserInputModel : NewUserInputModel
    {
        [DataType(DataType.Password)]
        new public string Password { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        public string Name => $"{FirstName} {LastName}";
    }
}
