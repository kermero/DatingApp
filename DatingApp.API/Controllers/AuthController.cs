using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Dtos;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegister userForRegister)
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
    }
}