using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Api.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public sealed class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("validate-alta")]
        public async Task<IActionResult> ValidateAlta([FromBody] ValidateAltaDto dto, CancellationToken cancellationToken)
        {
            var result = await _userService.ValidateNumeroAltaAsync(dto.NumeroAlta, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpPost("validate-contact")]
        public async Task<IActionResult> ValidateContact([FromBody] ValidateContactDto dto, CancellationToken cancellationToken)
        {
            var result = await _userService.ValidateContactAsync(dto.Phone, dto.Email, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto, CancellationToken cancellationToken)
        {
            var result = await _userService.SendOtpAsync(dto.Destination, dto.Channel, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess ? Ok() : StatusCode(500, result.Error);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto, CancellationToken cancellationToken)
        {
            var result = await _userService.VerifyOtpAsync(dto.Destination, dto.Channel, dto.Otp, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount([FromBody] ValidateContactDto dto, CancellationToken cancellationToken)
        {
            var result = await _userService.CreateUserAccountAsync(dto.Phone, dto.Email, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess ? CreatedAtAction(nameof(CreateAccount), new { userId = result.Value }) : BadRequest(result.Error);
        }

        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto dto, CancellationToken cancellationToken)
        {
            var result = await _userService.SetPasswordAsync(dto.UserId, dto.Password, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpPost("complete-registration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationDto dto, CancellationToken cancellationToken)
        {
            var result = await _userService.CompleteRegistrationAsync(dto, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }

        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordDto dto, CancellationToken cancellationToken)
        {
            var result = await _userService.RecoverPasswordAsync(dto.Destination, dto.Channel, cancellationToken).ConfigureAwait(false);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }
    }
}