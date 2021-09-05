using MessagingService.Api.Controllers;
using MessagingService.Api.Models.Auth;
using MessagingService.Api.Models.Common;
using MessagingService.Api.Services;
using MessagingService.DataAccess.Model;
using MessagingService.DataAccess.Repositories;
using MessagingService.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace MessagingService.Api.Test
{
    public class AuthControllerTest
    {
        private readonly Mock<ILogger> loggerMock;
        private readonly Mock<IAccountRepository> accountRepositoryMock;
        private readonly Mock<IPasswordService> passwordMock;
        private readonly Mock<IJwtService> jwtServiceMock;

        private readonly AuthController authController;//class under test

        public AuthControllerTest()
        {
            passwordMock = new Mock<IPasswordService>();
            jwtServiceMock = new Mock<IJwtService>();
            accountRepositoryMock = new Mock<IAccountRepository>();
            loggerMock = new Mock<ILogger>();
            authController = new AuthController(passwordMock.Object, accountRepositoryMock.Object, jwtServiceMock.Object, loggerMock.Object);
        }

        #region Login Tests
        [Fact]
        public void Login_Succesful()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "testuser",
                Password = "SuperSecretP@@ssWord111"
            };
            var hashedpassword = "hashedpassword";
            string userDisplayName = "Test User";
            accountRepositoryMock.
                Setup(q => q.GetSingleOrDefault(It.IsAny<Expression<Func<AccountModel, bool>>>())).
                Returns(new AccountModel
                {
                    DisplayName = userDisplayName,
                    PasswordHash = hashedpassword,
                    PasswordSalt = Convert.ToBase64String(Array.Empty<byte>())
                });
            string jwtToken = "jwtToken";
            jwtServiceMock.Setup(q => q.CreateToken(It.IsAny<Dictionary<string, string>>())).Returns(jwtToken);
            passwordMock.Setup(q => q.HashText(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(hashedpassword);


            var actionResult = authController.Login(loginRequest);

            var result = actionResult as OkObjectResult;
            var loginResponse = result.Value as LoginResponse;
            accountRepositoryMock.Verify(x => x.UpdateLastLogin(It.IsAny<string>()), Times.Once);
            loggerMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
            Assert.Equal(jwtToken, loginResponse.AccessToken);
            Assert.Equal(userDisplayName, loginResponse.DisplayName);
        }

        [Theory]
        [InlineData("withoutNumber")]
        [InlineData("withoutuppercase1")]
        [InlineData("WITHOUTLOWERCASE1")]
        public void Login_Fail_InvalidRequest(string password)
        {
            var loginRequest = new LoginRequest
            {
                UserName = "testuser",
                Password = password
            };
            var actionResult = authController.Login(loginRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var loginResponse = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);

            Assert.Equal("The username or password is incorrect.", loginResponse.Error);

        }

        [Fact]
        public void Login_Fail_AccountDoesntExit()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "testuser",
                Password = "SuperSecretP@@ssWord111"
            };

            accountRepositoryMock.
               Setup(q => q.GetSingleOrDefault(It.IsAny<Expression<Func<AccountModel, bool>>>())).
               Returns((AccountModel)null);

            var actionResult = authController.Login(loginRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var loginResponse = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);

            Assert.Equal("The username or password is incorrect.", loginResponse.Error);
        }

        [Fact]
        public void Login_Fail_InvalidPassword()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "testuser",
                Password = "SuperSecretP@@ssWord111"
            };
            var hashedpassword = "hashedpassword";

            accountRepositoryMock.
                Setup(q => q.GetSingleOrDefault(It.IsAny<Expression<Func<AccountModel, bool>>>())).
                Returns(new AccountModel
                {
                    DisplayName = "Test User",
                    UserName = "UserName",
                    PasswordHash = hashedpassword,
                    PasswordSalt = Convert.ToBase64String(Array.Empty<byte>()),
                    Id = new MongoDB.Bson.ObjectId("507f1f77bcf86cd799439011")
                });
            passwordMock.
                Setup(q => q.HashText(It.IsAny<string>(), It.IsAny<byte[]>())).
                Returns(string.Empty);

            var actionResult = authController.Login(loginRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var loginResponse = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);

            Assert.Equal("The username or password is incorrect.", loginResponse.Error);
        }

        [Fact]
        public void Login_Fail_UnexpectedError()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "testuser",
                Password = "SuperSecretP@@ssWord111"
            };

            accountRepositoryMock.
                Setup(q => q.GetSingleOrDefault(It.IsAny<Expression<Func<AccountModel, bool>>>())).
                Throws(new Exception("Unexpected error"));

            var actionResult = authController.Login(loginRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var loginResponse = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);

            Assert.Equal("Unable to process Request. Please contact to Admin.", loginResponse.Error);
        }

        #endregion

        #region Register Tests
        [Fact]
        public void Register_Succesful()
        {
            var registerRequest = new RegisterRequest
            {
                DisplayName = "Test User",
                UserName = "testuser",
                EMail = "test@user.com",
                Password = "SuperSecretP@@ssWord111"
            };

            passwordMock.
                Setup(q => q.HashText(It.IsAny<string>(), It.IsAny<byte[]>())).
                Returns(string.Empty);

            accountRepositoryMock.
                Setup(q => q.Create(It.IsAny<AccountModel>())).
                Returns(new AccountModel());

            var actionResult = authController.Register(registerRequest);

            loggerMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
            Assert.IsType<CreatedAtActionResult>(actionResult);
        }

        [Theory]
        [InlineData("withoutNumber", "Password must contain a digit.")]
        [InlineData("withoutuppercase1", "Password must contain a uppercase letter.")]
        [InlineData("WITHOUTLOWERCASE1", "Password must contain a lowercase letter.")]
        public void Register_Fail_InvalidRequest_InvalidPassword(string password, string error)
        {
            var registerRequest = new RegisterRequest
            {
                DisplayName = "Test User",
                UserName = "testuser",
                EMail = "test@user.com",
                Password = password
            };

            var actionResult = authController.Register(registerRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var loginResponse = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Contains(error, loginResponse.Error);
        }


        [Theory]
        [InlineData("xyz", "The length of 'User Name' must be at least 4 characters. You entered 3 characters.")]
        [InlineData("verylongusername", "The length of 'User Name' must be 12 characters or fewer. You entered 16 characters.")]
        public void Register_Fail_InvalidRequest_InvalidUserName(string username, string error)
        {
            var registerRequest = new RegisterRequest
            {
                DisplayName = "Test User",
                UserName = username,
                EMail = "test@user.com",
                Password = "SuperSecretP@@ssWord111"
            };

            var actionResult = authController.Register(registerRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var loginResponse = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Contains(error, loginResponse.Error);
        }

        [Theory]
        [InlineData("xyz", "The length of 'Display Name' must be at least 4 characters. You entered 3 characters.")]
        public void Register_Fail_InvalidRequest_InvalidDisplayName(string displayName, string error)
        {
            var registerRequest = new RegisterRequest
            {
                DisplayName = displayName,
                UserName = "username",
                EMail = "test@user.com",
                Password = "SuperSecretP@@ssWord111"
            };

            var actionResult = authController.Register(registerRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var loginResponse = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Contains(error, loginResponse.Error);
        }

        [Theory]
        [InlineData("xyz", "'E Mail' is not a valid email address.")]
        public void Register_Fail_InvalidRequest_InvalidEmail(string email, string error)
        {
            var registerRequest = new RegisterRequest
            {
                DisplayName = "Test User",
                UserName = "username",
                EMail = email,
                Password = "SuperSecretP@@ssWord111"
            };

            var actionResult = authController.Register(registerRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var loginResponse = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Contains(error, loginResponse.Error);
        }

        [Fact]
        public void Register_Fail_UnexpectedError()
        {
            var registerRequest = new RegisterRequest
            {
                DisplayName = "Test User",
                UserName = "testuser",
                EMail = "test@user.com",
                Password = "SuperSecretP@@ssWord111"
            };

            passwordMock.
                Setup(q => q.GenerateSalt()).
                Throws(new Exception("Unexpected error"));


            var actionResult = authController.Register(registerRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var loginResponse = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);

            Assert.Equal("Unable to process Request. Please contact to Admin.", loginResponse.Error);
        }
        #endregion
    }
}