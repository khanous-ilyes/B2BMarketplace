using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAccount _repository;

        public AuthController(IUserAccount repository)
        {
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto model)
        {
            if (model == null) return BadRequest("Model is empty");
            
            var result = await _repository.CreateAccount(model);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto model)
        {
            if (model == null) return BadRequest("Model is empty");
            
            var result = await _repository.SignIn(model);
            return Ok(result);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto model)
        {
             if (model == null) return BadRequest("Model is empty");
             var result = await _repository.SignInWithGoogle(model);
             return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (model == null) return BadRequest("Model is empty");
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            var result = await _repository.ChangePassword(userId, model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailDto model)
        {
            if (model == null) return BadRequest("Model is empty");
            var userId = GetUserId();
            if (userId == -1) return Unauthorized();

            var result = await _repository.ChangeEmail(userId, model);
            return Ok(result);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var result = await _repository.VerifyEmail(token);
            if (!result.Flag) return BadRequest(result.Message);
            return Ok(new { message = result.Message });
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return -1;
            return int.Parse(claim.Value);
        }
    }
}
