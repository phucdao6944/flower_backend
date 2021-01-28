using flower.Entities;
using flower.Helpers;
using flower.Models;
using flower.Repositories.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace flower.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            HttpContext context = Request.HttpContext;
            var userLogin = (User)context.Items["User"];
            return Ok(new ResponseForm<User>(userLogin));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddNewUserRequest addNewUserRequest)
        {
            try
            {
                await _userRepository.AddUser(addNewUserRequest);
                User user = await _userRepository.GetUserByUserName(addNewUserRequest.username);
                return Ok(new ResponseForm<User>(user));
            } catch (Exception e)
            {
                return StatusCode(400, new ResponseForm<object>(null, e.Message, 400));
            }
            
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateUserRequest updateUser)
        {
            try
            {
                HttpContext context = Request.HttpContext;
                var userLogin = (User)context.Items["User"];
                await _userRepository.UpdateUser(userLogin.id, updateUser);
                User user = await _userRepository.GetById(userLogin.id);
                return Ok(new ResponseForm<User>(user));
            }
            catch (Exception e)
            {
                return StatusCode(400, new ResponseForm<object>(null, e.Message, 400));
            }
        }
    }
}
