using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AuthenticationServer.API.Helpers;
using AuthenticationServer.API.Models;
using AuthenticationServer.API.Models.Responses;
using AuthenticationServer.API.Models.Request;
using AuthenticationServer.API.Services.Authenticators;
using AuthenticationServer.API.Services.Repositories;
using AuthenticationServer.API.Services.TokenValidators;

namespace AuthenticationServer.API.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly RefreshTokenValidator refreshTokenValidator;
        private readonly Authenticator authenticator;

        public AuthenticationController(IUserRepository userRepository, RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepository refreshTokenRepository, Authenticator authenticator)
        {
            this.userRepository = userRepository;
            this.refreshTokenValidator = refreshTokenValidator;
            this.refreshTokenRepository = refreshTokenRepository;
            this.authenticator = authenticator;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequestModelState();
            }

            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return BadRequest(new ErrorResponse("Password does not match confirm password."));
            }

            var existingUserByEmail = await this.userRepository.GetByEmail(registerRequest.Email);
            if (existingUserByEmail != null)
            {
                return Conflict(new ErrorResponse("Email already exists."));
            }

            var existingUserByUsername = await this.userRepository.GetByUserName(registerRequest.Username);
            if (existingUserByUsername != null)
            {
                return Conflict(new ErrorResponse("Username already exists."));
            }

            var registrationUser = new User()
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username,
                PasswordHash = registerRequest.Password.HashPassword()
            };

            await this.userRepository.Create(registrationUser);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!this.ModelState.IsValid)
            {
                this.BadRequestModelState();
            }

            var user = await this.userRepository.GetByUserName(loginRequest.Username);
            if (user is null)
            {
                return Unauthorized(new ErrorResponse("Username does not exist."));
            }

            if (!loginRequest.Password.VerifyPassword(user.PasswordHash))
            {
                return Unauthorized(new ErrorResponse("Password is incorrect."));
            }

            var response = await this.authenticator.AuthenticateUserAsync(user);
            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequestModelState();
            }

            if (!this.refreshTokenValidator.Validate(refreshRequest.RefreshTokenValue))
            {
                return this.BadRequest(new ErrorResponse("Invalid refresh token"));
            }

            var refreshToken = await this.refreshTokenRepository.GetByTokenValue(refreshRequest.RefreshTokenValue);
            if (refreshToken is null)
            {
                return NotFound(new ErrorResponse("Invalid refresh token"));
            }

            await this.refreshTokenRepository.Delete(refreshToken.Id);

            var user = await this.userRepository.GetById(refreshToken.UserId);
            if (user is null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }

            var response = await this.authenticator.AuthenticateUserAsync(user);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var unparsedUserId = HttpContext.User.FindFirstValue("id");

            if (!Guid.TryParse(unparsedUserId, out var userId))
            {
                return Unauthorized();
            }

            await this.refreshTokenRepository.DeleteAll(userId);

            return NoContent();
        }

        private IActionResult BadRequestModelState()
        {
            var errorMessage = this.ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ErrorResponse(errorMessage));
        }
    }
}
