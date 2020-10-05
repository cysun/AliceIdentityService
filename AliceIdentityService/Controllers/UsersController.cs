using System;
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
            user.Nickname = input.FirstName;
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                result = await _userManager.AddClaimsAsync(user, user.Claims());
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to add user claims", result.Errors);
                }

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
        public async Task<IActionResult> EditAsync(string id)
        {
            var user = _userService.GetUser(id);
            if (user == null) return NotFound();

            var claims = await _userManager.GetClaimsAsync(user);
            var userClaims = user.Claims();
            var userClaimTypes = userClaims.Select(c => c.Type).ToHashSet();

            ViewBag.UserClaims = userClaims;
            ViewBag.OtherClaims = claims.Where(c => !userClaimTypes.Contains(c.Type)).ToList();

            return View(_mapper.Map<EditUserInputModel>(user));
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(string id, EditUserInputModel input)
        {
            if (!ModelState.IsValid) return View(input);

            var user = _userService.GetUser(id);

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

            _mapper.Map(input, user);
            _userService.SaveChanges();

            await UpdateClaims(user);

            _logger.LogInformation("{user} edited account {account}", User.Identity.Name, input.Email);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Users/{userId}/Claims/{claimType}={claimValue}")]
        public async Task<IActionResult> AddClaimAsync(string userId, string claimType, string claimValue)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = _userService.GetUser(userId);

            var result = await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
            if (result.Succeeded)
            {
                _logger.LogError("{user} added claim {claimType}={claimValue} to {account}",
                    User.Identity.Name, claimType, claimValue, userId);
                return Ok();
            }
            else
            {
                _logger.LogError("Failed to add claim {claimType}={claimValue} to {account}",
                        claimType, claimValue, userId, result.Errors);
                return StatusCode(500);
            }
        }

        [HttpDelete("Users/{userId}/Claims/{claimType}={claimValue}")]
        public async Task<IActionResult> RemoveClaimAsync(string userId, string claimType, string claimValue)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = _userService.GetUser(userId);

            var result = await _userManager.RemoveClaimAsync(user, new Claim(claimType, claimValue));
            if (result.Succeeded)
            {
                _logger.LogError("{user} removed claim {claimType}={claimValue} from {account}",
                    User.Identity.Name, claimType, claimValue, userId);
                return Ok();
            }
            else
            {
                _logger.LogError("Failed to remove claim {claimType}={claimValue} from {account}",
                        claimType, claimValue, userId, result.Errors);
                return StatusCode(500);
            }
        }

        private async Task<IdentityResult> ChangePaswordAsync(User user, string newPassword)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        private async Task UpdateClaims(User user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var claimsToAdd = new List<Claim>();
            var claimsToRemove = new List<Claim>();
            foreach (var userClaim in user.Claims())
            {
                var oldUserClaim = claims.Where(c => c.Type == userClaim.Type).SingleOrDefault();
                if (oldUserClaim == null)
                    claimsToAdd.Add(userClaim);
                else if (oldUserClaim.Value != userClaim.Value)
                {
                    claimsToRemove.Add(oldUserClaim);
                    claimsToAdd.Add(userClaim);
                }
            }

            var result = await _userManager.RemoveClaimsAsync(user, claimsToRemove);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to remove user claims", result.Errors);
            }

            result = await _userManager.AddClaimsAsync(user, claimsToAdd);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to add user claims", result.Errors);
            }
        }
    }
}

namespace AliceIdentityService.Models
{
    public class NewUserInputModel : RegistrationInputModel
    {
        [Display(Name = "Administrator")]
        public bool IsAdministrator { get; set; }
    }

    public class EditUserInputModel : NewUserInputModel
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        new public string Password { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        public string Name => $"{FirstName} {LastName}";
    }
}
