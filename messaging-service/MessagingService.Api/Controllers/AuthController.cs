using MessagingService.Api.Models.Auth;
using MessagingService.Api.Services;
using MessagingService.DataAccess.Collection;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace MessagingService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IPasswordService passwordService;
        private readonly IAccountRepository accountRepository;
        private readonly IJwtService jwtService;

        public AuthController(IPasswordService passwordService, IAccountRepository accountRepository, IJwtService jwtService)
        {
            this.passwordService = passwordService;
            this.accountRepository = accountRepository;
            this.jwtService = jwtService;
        }


        [HttpPost(nameof(Login))]
        [ProducesResponseType(typeof(LoginResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Login(LoginRequest request)
        {
            var validationResult = request.IsValid();
            if (validationResult.Key == false)
            {
                return BadRequest(new
                {
                    errors = new string[]
                    {
                        "The username or password is incorrect."
                    }
                });
            }

            try
            {
                var account = accountRepository.GetSingleOrDefault(x => x.UserName == request.UserName);
                if (account is null)
                {
                    return BadRequest(new
                    {
                        errors = new string[]
                        {
                            "The username or password is incorrect."
                        }
                    });
                }

                var hashedPassword = passwordService.HashText(request.Password, Convert.FromBase64String(account.PasswordSalt));
                if (hashedPassword != account.PasswordHash)
                {
                    return BadRequest(new
                    {
                        errors = new string[]
                        {
                            "The username or password is incorrect."
                        }
                    });
                }

                Dictionary<string, string> claims = new Dictionary<string, string>();
                claims.Add("Id", account.Id.ToString());
                claims.Add("UserName", account.UserName);

                var token = jwtService.CreateToken(claims);

                return Ok(new LoginResult
                {
                    AccessToken = token,
                    DisplayName = account.DisplayName
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    errors = new string[]
                    {
                        $"Critical unexpected error.({ex.Message})"
                    }
                });
            }
        }

        [HttpPost(nameof(Register))]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Register(RegisterRequest request)
        {
            var validationResult = request.IsValid();
            if (validationResult.Key == false)
            {
                return BadRequest(new
                {
                    errors = validationResult.Value
                });
            }

            try
            {
                var passwordSalt = passwordService.GenerateSalt();
                var hashedPassword = passwordService.HashText(request.Password, passwordSalt);

                var account = accountRepository.Create(new DataAccess.Model.AccountModel
                {
                    DisplayName = request.DisplayName,
                    EMail = request.EMail,
                    PasswordSalt = Convert.ToBase64String(passwordSalt),
                    PasswordHash = hashedPassword,
                    UserName = request.UserName
                });

                return CreatedAtAction(nameof(Register), new { id = account.Id }, account);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    errors = new string[]
                    {
                        $"Critical unexpected error.({ex.Message})"
                    }
                });
            }
        }
    }

}