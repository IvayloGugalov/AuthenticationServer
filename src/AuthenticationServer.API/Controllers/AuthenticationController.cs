using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AuthenticationServer.API.Helpers;
using AuthenticationServer.API.Models;
using AuthenticationServer.API.Models.Request;
using AuthenticationServer.API.Services.Repositories;
using AuthenticationServer.API.Models.Responses;
using AuthenticationServer.API.Services.TokenGenerators;

namespace AuthenticationServer.API.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly AccessTokenGenerator accessTokenGenerator;

        public AuthenticationController(IUserRepository userRepository, AccessTokenGenerator accessTokenGenerator)
        {
            this.userRepository = userRepository;
            this.accessTokenGenerator = accessTokenGenerator;
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

            var accessToken = this.accessTokenGenerator.GenerateToken(user);

            return Ok(new UserAuthenticatedResponse()
            {
                AccessToken = accessToken
            });
        }

        private IActionResult BadRequestModelState()
        {
            var errorMessage = this.ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ErrorResponse(errorMessage));
        }
    }
}
