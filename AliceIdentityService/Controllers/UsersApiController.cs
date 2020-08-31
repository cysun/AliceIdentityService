using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliceIdentityService.Models;
using AliceIdentityService.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AliceIdentityService.Controllers
{
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly UserService _userService;

        private readonly IMapper _mapper;

        public UsersApiController(UserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("Users/PrefixSearch")]
        public List<UserViewModel> PrefixSearch([FromQuery] string q)
        {
            return _mapper.Map<List<User>, List<UserViewModel>>(_userService.SearchUsersByPrefix(q));
        }
    }
}

namespace AliceIdentityService.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
