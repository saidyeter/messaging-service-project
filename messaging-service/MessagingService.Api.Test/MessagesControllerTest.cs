using MessagingService.Api.Controllers;
using MessagingService.Api.Models.Common;
using MessagingService.Api.Models.Message;
using MessagingService.DataAccess.Model;
using MessagingService.DataAccess.Repositories;
using MessagingService.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace MessagingService.Api.Test
{
    public class MessagesControllerTest
    {
        private readonly Mock<ILogger> loggerMock;
        private readonly Mock<IAccountRepository> accountRepositoryMock;
        private readonly Mock<IMessageRepository> messageRepositoryMock;
        private readonly Mock<HttpContext> contextMock;
        private readonly Mock<HttpRequest> requestMock;

        private MessagesController messagesController;//class under test

        public MessagesControllerTest()
        {
            messageRepositoryMock = new Mock<IMessageRepository>();
            accountRepositoryMock = new Mock<IAccountRepository>();
            loggerMock = new Mock<ILogger>();
            messagesController = new MessagesController(messageRepositoryMock.Object, loggerMock.Object, accountRepositoryMock.Object);
            contextMock = new Mock<HttpContext>();
            requestMock = new Mock<HttpRequest>();
        }

        #region Send Message Tests

        [Fact]
        public void SendMessage_Succesful()
        {
            var sendRequest = new SendRequest
            {
                Message = "message",
                ReceiverUser = "receiveruser"
            };

            accountRepositoryMock.
                Setup(x => x.DoesExist(It.IsAny<Expression<Func<AccountModel, bool>>>())).
                Returns(true);

            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };

            accountRepositoryMock.
                Setup(x => x.IsBlocked(It.IsAny<string>(), It.IsAny<string>())).
                Returns(false);
            messageRepositoryMock.
                Setup(x => x.Create(It.IsAny<MessageModel>())).
                Returns(new MessageModel());

            var actionResult = messagesController.SendMessage(sendRequest);
            Assert.IsType<CreatedAtActionResult>(actionResult);
        }

        [Theory]
        [InlineData("", "'Message' must not be empty.")]
        public void SendMessage_Fail_InvalidRequest_InvalidMessage(string message, string error)
        {
            var sendRequest = new SendRequest
            {
                Message = message,
                ReceiverUser = "receiveruser"
            };

            var actionResult = messagesController.SendMessage(sendRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Contains(error, response.Error);
        }

        [Theory]
        [InlineData("xyz", "The length of 'Receiver User' must be at least 4 characters. You entered 3 characters.")]
        [InlineData("verylongusername", "The length of 'Receiver User' must be 12 characters or fewer. You entered 16 characters.")]
        public void SendMessage_Fail_InvalidRequest_InvalidReceiverUser(string receiverUser, string error)
        {
            var sendRequest = new SendRequest
            {
                Message = "message",
                ReceiverUser = receiverUser
            };

            var actionResult = messagesController.SendMessage(sendRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Contains(error, response.Error);
        }

        [Fact]
        public void SendMessage_Fail_ReceiverUserDoesExist()
        {
            var sendRequest = new SendRequest
            {
                Message = "message",
                ReceiverUser = "receiverUser"
            };

            accountRepositoryMock.
                Setup(x => x.DoesExist(It.IsAny<Expression<Func<AccountModel, bool>>>())).
                Returns(false);

            var actionResult = messagesController.SendMessage(sendRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Equal("Receiver account doesn't exist.", response.Error);
        }

        [Fact]
        public void SendMessage_Fail_SenderAccountCouldntIdentify()
        {
            var sendRequest = new SendRequest
            {
                Message = "message",
                ReceiverUser = "receiveruser"
            };

            accountRepositoryMock.
                Setup(x => x.DoesExist(It.IsAny<Expression<Func<AccountModel, bool>>>())).
                Returns(true);

            var headers = new HeaderDictionary(new Dictionary<String, StringValues>()) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };

            var actionResult = messagesController.SendMessage(sendRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Equal("Unable to process Request. Please contact to Admin.", response.Error);
        }

        [Fact]
        public void SendMessageFail_SenderAccount_Blocked()
        {
            var sendRequest = new SendRequest
            {
                Message = "message",
                ReceiverUser = "receiveruser"
            };

            accountRepositoryMock.
                Setup(x => x.DoesExist(It.IsAny<Expression<Func<AccountModel, bool>>>())).
                Returns(true);


            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);

            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };

            accountRepositoryMock.
                Setup(x => x.IsBlocked(It.IsAny<string>(), It.IsAny<string>())).
                Returns(true);

            var actionResult = messagesController.SendMessage(sendRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
            Assert.Equal("Receiver account blocked you.", response.Error);
        }

        [Fact]
        public void SendMessage_Fail_UnexpectedError()
        {
            var sendRequest = new SendRequest
            {
                Message = "message",
                ReceiverUser = "receiveruser"
            };

            accountRepositoryMock.
                Setup(x => x.DoesExist(It.IsAny<Expression<Func<AccountModel, bool>>>())).
                Throws(new Exception("Unexpected error"));

            var actionResult = messagesController.SendMessage(sendRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);

            Assert.Equal("Unable to process Request. Please contact to Admin.", response.Error);
        }
        #endregion

        #region Get Message Tests
        [Fact]
        public void GetMessage_Succesful()
        {
            string messageId = "messageId";
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };
            var message = new MessageModel
            {
                Message = "message",
                ReceiverUser = "receiverUser",
                SenderUser = "senderUser",
            };
            messageRepositoryMock.
               Setup(x => x.GetSingleMessage(It.IsAny<string>(), It.IsAny<string>())).
               Returns(message);

            var actionResult = messagesController.GetMessage(messageId);
            Assert.IsType<OkObjectResult>(actionResult);

            var result = actionResult as OkObjectResult;
            var response = result.Value as MessageDTO;

            Assert.Equal(message.SenderUser, response.SenderUser);
            Assert.Equal(message.ReceiverUser, response.ReceiverUser);
            Assert.Equal(message.Message, response.Message);
        }

        [Fact]
        public void GetMessage_Fail_SenderAccountCouldntIdentify()
        {
            string messageId = "messageId";
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>()) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };

            var actionResult = messagesController.GetMessage(messageId);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Equal("Unable to process Request. Please contact to Admin.", response.Error);
        }

        [Fact]
        public void GetMessage_Fail_CouldntFindMessage()
        {
            string messageId = "messageId";
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };
            messageRepositoryMock.
              Setup(x => x.GetSingleMessage(It.IsAny<string>(), It.IsAny<string>())).
              Throws(new Exception("Couldn't find message"));

            var actionResult = messagesController.GetMessage(messageId);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
            Assert.Equal("Couldn't find message.", response.Error);
        }

        #endregion

        #region Last Messages From Tests
        [Fact]
        public void LastMessagesFrom_Succesful()
        {
            string messageId = "messageId";
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };
            List<string> messageIdList = new List<string>
            {
                "messageId1",
                "messageId2",
            };
            messageRepositoryMock.
                Setup(x => x.GetOlderMessages(It.IsAny<string>(), It.IsAny<string>())).
                Returns(messageIdList);


            var actionResult = messagesController.LastMessagesFrom(messageId);
            Assert.IsType<OkObjectResult>(actionResult);

            var result = actionResult as OkObjectResult;
            var response = result.Value as LastMessagesResponse;
            Assert.Equal(messageIdList, response.MessageIdList);

        }

        [Fact]
        public void LastMessagesFrom_Fail_SenderAccountCouldntIdentify()
        {
            string messageId = "messageId";
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>()) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };

            var actionResult = messagesController.LastMessagesFrom(messageId);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Equal("Unable to process Request. Please contact to Admin.", response.Error);
        }

        [Fact]
        public void LastMessagesFrom_Fail_CouldntGetMessages()
        {
            string messageId = "messageId";
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };
            messageRepositoryMock.
              Setup(x => x.GetOlderMessages(It.IsAny<string>(), It.IsAny<string>())).
              Throws(new Exception("Couldn't get messages"));

            var actionResult = messagesController.LastMessagesFrom(messageId);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
            Assert.Equal("Couldn't get messages.", response.Error);
        }

        #endregion


        #region Latest Message Between Tests

        [Fact]
        public void LatestMessageBetween_Succesfull()
        {
            string opponent = "opponent";
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };
            var messageId = "messageId";
            messageRepositoryMock.
               Setup(x => x.GetLatestMessage(It.IsAny<string>(), It.IsAny<string>())).
               Returns(messageId);

            var actionResult = messagesController.LatestMessageBetween(opponent);
            Assert.IsType<OkObjectResult>(actionResult);

            var result = actionResult as OkObjectResult;
            var response = result.Value as LatestMessageResponse;

            Assert.Equal(messageId, response.MessageId);
        }

        [Fact]
        public void LatestMessageBetween_Fail_SenderAccountCouldntIdentify()
        {
            string messageId = "messageId";
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>()) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };

            var actionResult = messagesController.LatestMessageBetween(messageId);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Equal("Unable to process Request. Please contact to Admin.", response.Error);
        }

        [Fact]
        public void LatestMessageBetween_Fail_CouldntFindMessage()
        {
            string messageId = "messageId";
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };
            messageRepositoryMock.
              Setup(x => x.GetLatestMessage(It.IsAny<string>(), It.IsAny<string>())).
              Throws(new Exception("Couldn't find message"));

            var actionResult = messagesController.LatestMessageBetween(messageId);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
            Assert.Equal("Couldn't find message.", response.Error);
        }

        #endregion

        #region Block User Tests

        [Fact]
        public void BlockUser_Succesful()
        {
            var blockUserRequest = new BlockUserRequest
            {
                OpponentUser = "OpponentUser"
            };
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };

            var actionResult = messagesController.BlockUser(blockUserRequest);
            Assert.IsType<OkResult>(actionResult);
            accountRepositoryMock.Verify(x => x.BlockUser(It.IsAny<string>(), It.IsAny<string>()),Times.Once);
        }

        [Fact]
        public void BlockUser_Fail_SenderAccountCouldntIdentify()
        {
            var blockUserRequest = new BlockUserRequest
            {
                OpponentUser = "OpponentUser"
            }; 
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>()) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };

            var actionResult = messagesController.BlockUser(blockUserRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
            Assert.Equal("Unable to process Request. Please contact to Admin.", response.Error);
        }

        [Fact]
        public void BlockUser_Fail_CouldntBlockUser()
        {
            var blockUserRequest = new BlockUserRequest
            {
                OpponentUser = "opponentUser"
            };
            var headers = new HeaderDictionary(new Dictionary<String, StringValues>
            {
                { "UserName", "senderusername" },
                { "Id", "senderuserid" },
            }) as IHeaderDictionary;
            requestMock.Setup(x => x.Headers).Returns(headers);
            contextMock.Setup(x => x.Request).Returns(requestMock.Object);
            messagesController.ControllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };
            accountRepositoryMock.
              Setup(x => x.BlockUser(It.IsAny<string>(), It.IsAny<string>())).
              Throws(new Exception("Couldn't block user"));

            var actionResult = messagesController.BlockUser(blockUserRequest);
            Assert.IsType<BadRequestObjectResult>(actionResult);
            var result = actionResult as BadRequestObjectResult;
            var response = result.Value as BadRequestResponse;

            loggerMock.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
            Assert.Equal("Couldn't block user.", response.Error);
        }

        #endregion
    }
}