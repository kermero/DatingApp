using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister userForRegister)
        {

            userForRegister.UserName = userForRegister.UserName.ToLower();
            if (await _repo.UserExists(userForRegister.UserName))
                return BadRequest("Username Already existx!");
            var userToCreate = new User
            {
                UserName = userForRegister.UserName
            };
            var createdUser = await _repo.Register(userToCreate, userForRegister.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var UserFromRepo = await _repo.Login(userForLoginDto.UserName.ToLower(), userForLoginDto.Password);
            if (UserFromRepo == null)
                return Unauthorized();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, UserFromRepo.UserName)

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds  

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });


        }
    }
}