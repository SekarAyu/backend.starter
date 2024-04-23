using backend.starter.Common.Commands;
using backend.starter.Common.Dtos;
using backend.starter.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace backend.starter.Api.AspNetCore.Controller
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        [Route("profile")]
        public void RegisterNewProfile([FromBody] AddUserProfileCommand command)
        {
            userService.AddNewProfile(command);
        }

        [HttpGet]
        [Route("all/profile")]
        public IList<UserProfileDto> GetAllUserProfile()
        {
            return userService.GetAllUserProfile();
        }
    }
}
