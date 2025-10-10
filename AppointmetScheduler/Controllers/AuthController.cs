using AppointmetScheduler.Entities;
using AppointmetScheduler.Models;
using AppointmetScheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmetScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthServices _authServices;
        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegDto request)
        {
            var user = await _authServices.RegisterAsync(request);
            if (user == null)
            {
                return BadRequest("Username already exists");
            }
            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var result = await _authServices.LoginAsync(request);
            if (result == null)
            {
                return BadRequest("Invalid Username or Password");
            }

            return Ok(result);
        }
        [HttpGet("protected")]
        [Authorize]
        public ActionResult get()
        {
            return Ok("You are logged in");
        }
        [HttpGet("admin")]
        [Authorize(Roles ="Admin")]
        public ActionResult getAdmin()
        {
            return Ok("You are admin");
        }
    }
}
