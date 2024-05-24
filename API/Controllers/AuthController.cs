using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserProduct.Api.Dto;
using UserProduct.Api.Extensions;
using UserProduct.Core.Abstractions;
using UserProduct.Core.Dtos;

namespace UserProduct.Api.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerUserDto)
        {
            var result = await _authService.Register(registerUserDto);

            if (result.IsFailure)
                return BadRequest(ResponseDto<object>.Failure(result.Errors));

            return Ok(ResponseDto<object>.Success());
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var result = await _authService.Login(loginUserDto);

            if (result.IsFailure)
                return BadRequest(ResponseDto<object>.Failure(result.Errors));

            return Ok(ResponseDto<object>.Success(result.Data));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseDto<object>.Failure(ModelState.GetErrors()));
            }

            var resetPasswordResult = await _authService.ResetPasswordAsync(resetPasswordDto);

            if (resetPasswordResult.IsFailure)
                return BadRequest(ResponseDto<object>.Failure(resetPasswordResult.Errors));

            return Ok(ResponseDto<object>.Success(resetPasswordResult));
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _authService.ForgotPassword(resetPasswordDto);

            if (result.IsFailure)
                return BadRequest(ResponseDto<object>.Failure(result.Errors));

            return Ok(ResponseDto<object>.Success());
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmail(email, token);

            if (result.IsFailure)
                return BadRequest(ResponseDto<object>.Failure(result.Errors));

            return Ok(ResponseDto<object>.Success());
        }
    }
}
