using flower.Helpers;
using flower.Models;
using flower.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace flower.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        private readonly IUserRepository _userRepository;

        public AuthController(IOptions<AppSettings> appSettings, IUserRepository userRepository)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            var user = await _userRepository.FindUserByUserNameAndPassWord(model.username, model.password);
            if (user == null || (model.admin && !user.is_admin))
                return BadRequest(new { message = "Username or password is incorrect" });

            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.secret);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim("id", user.id.ToString())
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new AuthenticateResponse(user, tokenHandler.WriteToken(token)));
        }
    }
}
