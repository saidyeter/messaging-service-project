using MessagingService.Api.Helpers;
using MessagingService.Api.Models.Auth;
using MessagingService.Api.Models.Common;
using MessagingService.Api.Services;
using MessagingService.DataAccess.Repositories;
using MessagingService.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

namespace MessagingService.Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IPasswordService passwordService;
        private readonly IAccountRepository accountRepository;
        private readonly IJwtService jwtService;
        private readonly ILogger logger;

        public AuthController(IPasswordService passwordService, IAccountRepository accountRepository, IJwtService jwtService, ILogger logger)
        {
            this.passwordService = passwordService;
            this.accountRepository = accountRepository;
            this.jwtService = jwtService;
            this.logger = logger;
        }


        [HttpPost(nameof(Login))]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Login(LoginRequest request)
        {
            var validationResult = request.IsValid();
            if (validationResult.Key == false)
            {
                logger.Error(validationResult.Value) ;
                return BadRequest(new BadRequestResponse("The username or password is incorrect."));
            }

            try
            {
                var account = accountRepository.GetSingleOrDefault(x => x.UserName == request.UserName);
                if (account is null)
                {
                    logger.Error($"Account doesn't exist. Request : '{System.Text.Json.JsonSerializer.Serialize(request)}'");
                    return BadRequest(new BadRequestResponse("The username or password is incorrect."));
                }

                var hashedPassword = passwordService.HashText(request.Password, Convert.FromBase64String(account.PasswordSalt));
                if (hashedPassword != account.PasswordHash)
                {
                    logger.Error($"Invalid password. Request : '{System.Text.Json.JsonSerializer.Serialize(request)}'");
                    return BadRequest(new BadRequestResponse("The username or password is incorrect."));
                }

                Dictionary<string, string> claims = new Dictionary<string, string>();
                claims.Add("Id", account.Id.ToString());
                claims.Add("UserName", account.UserName);
                var token = jwtService.CreateToken(claims);
                
                accountRepository.UpdateLastLogin(account.Id.ToString());

                logger.Info($"Login Succeed for '{account.UserName}'.");

                return Ok(new LoginResponse
                {
                    AccessToken = token,
                    DisplayName = account.DisplayName
                });
            }
            catch (Exception ex)
            {
                logger.Error($"Unexpected error on Login. Request : '{System.Text.Json.JsonSerializer.Serialize(request)}'", ex);
                return BadRequest(new BadRequestResponse("Unable to process Request. Please contact to Admin."));
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
                logger.Error(validationResult.Value);
                return BadRequest(new BadRequestResponse(validationResult.Value));
            }

            try
            {

                if (accountRepository.DoesExist(x => x.UserName == request.UserName))
                {
                    logger.Error("Username exists.");
                    return BadRequest(new BadRequestResponse("Username exists."));
                }
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

                logger.Info($"Register Succeed for '{account.UserName}'.");
                return CreatedAtAction(nameof(Register), new { id = account.Id.ToString() });
            }
            catch (Exception ex)
            {
                logger.Error($"Unexpected error on Register. Request : '{System.Text.Json.JsonSerializer.Serialize(request)}'", ex);
                return BadRequest(new BadRequestResponse("Unable to process Request. Please contact to Admin."));
            }
        }
    }

}