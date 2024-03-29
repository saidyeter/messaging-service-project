﻿using MessagingService.Api.Helpers;
using MessagingService.Api.Models.Common;
using MessagingService.Api.Models.Message;
using MessagingService.DataAccess.Model;
using MessagingService.DataAccess.Repositories;
using MessagingService.Logging;
using MessagingService.MessagePublisher;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace MessagingService.Api.Controllers
{
    [JwtAuthorize]
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository messageRepository;
        private readonly ILogger logger;
        private readonly IAccountRepository accountRepository;
        private readonly IMessagePublisher messagePublisher;

        public MessagesController(IMessageRepository messageRepository, ILogger logger, IAccountRepository accountRepository, IMessagePublisher messagePublisher)
        {
            this.messageRepository = messageRepository;
            this.logger = logger;
            this.accountRepository = accountRepository;
            this.messagePublisher = messagePublisher;
        }


        /// <summary> sends message to opponent </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SendMessage")]
        [ProducesResponseType(typeof(SendResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult SendMessage(SendRequest request)
        {
            //TODO: event fırlat

            var validationResult = request.IsValid();
            if (validationResult.Key == false)
            {
                logger.Error(validationResult.Value);
                return BadRequest(new BadRequestResponse(validationResult.Value));
            }

            try
            {
                if (!accountRepository.DoesExist(x => x.UserName == request.ReceiverUser))
                {
                    logger.Error($"Receiver account doesn't exist. Request : '{System.Text.Json.JsonSerializer.Serialize(request)}'");
                    return BadRequest(new BadRequestResponse("Receiver account doesn't exist."));
                }

                var usernametaken = Request.Headers.TryGetValue("UserName", out Microsoft.Extensions.Primitives.StringValues SenderUserName);
                var userIdTaken = Request.Headers.TryGetValue("Id", out Microsoft.Extensions.Primitives.StringValues SenderUserId);
                if (!usernametaken || !userIdTaken)
                {
                    logger.Error($"Sender account couldn't identify. Request : '{System.Text.Json.JsonSerializer.Serialize(request)}'");
                    return BadRequest(new BadRequestResponse("Unable to process Request. Please contact to Admin."));
                }

                if (accountRepository.IsBlocked(SenderUserId.ToString(), request.ReceiverUser))
                {
                    logger.Info($"Receiver account blocked. Request : '{System.Text.Json.JsonSerializer.Serialize(request)}'");
                    return BadRequest(new BadRequestResponse("Receiver account blocked you."));
                }


                var createResult = messageRepository.Create(new MessageModel
                {
                    Message = request.Message,
                    ReceiverUser = request.ReceiverUser,
                    SenderUser = SenderUserName.ToString(),
                    SendedAt = System.DateTime.UtcNow,

                });
                accountRepository.UpdateLastMessage(createResult.SenderUser, createResult.ReceiverUser,createResult.Message);
                
                var channel = "ch";
                messagePublisher.PublishMessage(channel, createResult.Id.ToString(), createResult.SenderUser, createResult.ReceiverUser);

                return CreatedAtAction(nameof(SendMessage), new SendResponse
                {
                    MessageId = createResult.Id.ToString()
                });
            }
            catch (Exception ex)
            {
                logger.Error($"Unexpected error on Send. Request : '{System.Text.Json.JsonSerializer.Serialize(request)}'", ex);
                return BadRequest(new BadRequestResponse("Unable to process Request. Please contact to Admin."));
            }
        }


        /// <summary> retrieves single message that has id specified in parameters </summary>
        /// <param name="messageId"> id of message </param>
        /// <returns> message detail </returns>
        [HttpGet("GetMessage/{messageId}")]
        [ProducesResponseType(typeof(MessageDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult GetMessage(string messageId)
        {
            var usernametaken = Request.Headers.TryGetValue("UserName", out Microsoft.Extensions.Primitives.StringValues SenderUserName);
            if (!usernametaken)
            {
                logger.Error($"Sender account couldn't identify. id : '{messageId}'");
                return BadRequest(new BadRequestResponse("Unable to process Request. Please contact to Admin."));
            }

            MessageDTO message;
            try
            {
                message = messageRepository.GetSingleMessage(messageId, SenderUserName.ToString()).AsDTO();
                if(message.ReceiverUser == SenderUserName.ToString()){
                    messageRepository.SetSeen(messageId);
                }                    
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return BadRequest(new BadRequestResponse("Couldn't find message."));
            }
            return Ok(message);
        }


        /// <summary> retrieves last certain amount messages from message that has id specified in parameters </summary>
        /// <param name="messageId"> id of message that start point to retrospective dialog </param>
        /// <returns> found message id list </returns>
        [HttpGet("GetOlderMessagesFrom/{messageId}")]
        [ProducesResponseType(typeof(LastMessagesResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult GetOlderMessagesFrom(string messageId)
        {
            var usernametaken = Request.Headers.TryGetValue("UserName", out Microsoft.Extensions.Primitives.StringValues SenderUserName);
            if (!usernametaken)
            {
                logger.Error($"Sender account couldn't identify. id : '{messageId}'");
                return BadRequest(new BadRequestResponse("Unable to process Request. Please contact to Admin."));
            }

            List<string> messageIdList;
            try
            {
                messageIdList = messageRepository.GetOlderMessages(messageId, SenderUserName.ToString());

            }
            catch (Exception ex)
            {
                logger.Error($"Couldn't get messages. id : {messageId}", ex);
                return BadRequest(new BadRequestResponse("Couldn't get messages."));
            }

            return Ok(new LastMessagesResponse
            {
                MessageIdList = messageIdList.ToArray()
            });
        }


        /// <summary> retrieves latest message between signed in user and opponent user for start point </summary>
        /// <param name="opponent"> user messaging with </param>
        /// <returns> latest message id </returns>
        [HttpGet("GetLatestMessageBetween/{opponent}")]
        [ProducesResponseType(typeof(LatestMessageResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult GetLatestMessageBetween(string opponent)
        {
            var usernametaken = Request.Headers.TryGetValue("UserName", out Microsoft.Extensions.Primitives.StringValues SenderUserName);
            if (!usernametaken)
            {
                logger.Error($"Sender account couldn't identify. opponent :'{opponent}'");
                return BadRequest(new BadRequestResponse("Unable to process Request. Please contact to Admin."));
            }

            string messageId;
            try
            {
                messageId = messageRepository.GetLatestMessage(SenderUserName.ToString(), opponent);
            }
            catch (Exception ex)
            {
                logger.Error($"Couldn't find message.({ex.Message})", ex);
                return BadRequest(new BadRequestResponse("Couldn't find message."));
            }

            return Ok(new LatestMessageResponse
            {
                MessageId = messageId
            });
        }


        /// <summary> blocks opponent user to send message </summary>
        /// <param name="request"> user messaging with </param>
        [HttpPost("​BlockUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult BlockUser(BlockUserRequest request)
        {
            var validationResult = request.IsValid();
            if (validationResult.Key == false)
            {
                logger.Error(validationResult.Value);
                return BadRequest(new BadRequestResponse(validationResult.Value));
            }

            var userIdTaken = Request.Headers.TryGetValue("Id", out Microsoft.Extensions.Primitives.StringValues SenderUserId);
            if (!userIdTaken)
            {
                logger.Error($"Sender account couldn't identify. opponent :'{request.Opponent}'");
                return BadRequest(new BadRequestResponse("Unable to process Request. Please contact to Admin."));
            }

            try
            {
                accountRepository.BlockUser(SenderUserId.ToString(), request.Opponent);
            }
            catch (Exception ex)
            {
                logger.Error($"Couldn't block user.({ex.Message})", ex);
                return BadRequest(new BadRequestResponse("Couldn't block user."));
            }
            return Ok();
        }



        /// <summary> retrieves latest message between signed in user and opponent user for start point </summary>
        /// <param name="opponent"> user messaging with </param>
        /// <returns> latest message id </returns>
        [HttpGet("GetOpponents")]
        [ProducesResponseType(typeof(ChattedUser[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult GetOpponents()
        {
            var userIdTaken = Request.Headers.TryGetValue("Id", out Microsoft.Extensions.Primitives.StringValues SenderUserId);
            if (!userIdTaken)
            {
                logger.Error($"Sender account couldn't identify.");
                return BadRequest(new BadRequestResponse("Unable to process Request. Please contact to Admin."));
            }

            ChattedUser[] account;
            try
            {
                var senderAccount = accountRepository.GetById(SenderUserId.ToString());
                account = senderAccount.ChattedUsers.ToArray();
            }
            catch (Exception ex)
            {
                logger.Error($"Couldn't find opponents.({ex.Message})", ex);
                return BadRequest(new BadRequestResponse("Couldn't find opponents."));
            }

            return Ok(account);
        }


        /// <summary> retrieves latest message between signed in user and opponent user for start point </summary>
        /// <param name="opponent"> user messaging with </param>
        /// <returns> latest message id </returns>
        [HttpGet("GetUserInfo/{opponent}")]
        [ProducesResponseType(typeof(AccountDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult GetUserInfo(string opponent)
        {
            AccountDTO account;
            try
            {
                account= accountRepository.GetFirstOrDefault(x=> x.UserName == opponent).AsDTO();
            }
            catch (Exception ex)
            {
                logger.Error($"Couldn't find account.({ex.Message})", ex);
                return BadRequest(new BadRequestResponse("Couldn't find account."));
            }

            return Ok(account);
        }
    }

}

