using FluentValidation;
using MessagingService.Api.Models.Message;
using System.Collections.Generic;
using System.Linq;

namespace MessagingService.Api.Helpers
{
    public static class Extensions
    {
        public static KeyValuePair<bool, string> IsValid<T>(this AbstractValidator<T> validator, T obj)
        {
            var validationResult = validator.Validate(obj);
            if (validationResult.IsValid)
            {
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            else
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToArray();
                var errorMessage = $"Invalid {typeof(T)} Request : {string.Join(',', errors)}. Request : '{System.Text.Json.JsonSerializer.Serialize(obj)}'";
                return new KeyValuePair<bool, string>(false, errorMessage);
            }
        }

        public static MessageDTO AsDTO(this DataAccess.Model.MessageModel messageEntity)
        {
            return new MessageDTO
            {
                Id = messageEntity.Id.ToString(),
                Message = messageEntity.Message,
                SenderUser = messageEntity.SenderUser,
                ReceiverUser = messageEntity.ReceiverUser,
                SeenAt = messageEntity.SeenAt,
                SendedAt = messageEntity.SeenAt,
            };
        }
        public static AccountDTO AsDTO(this DataAccess.Model.AccountModel accountEntity)
        {
            return new AccountDTO
            {
                DisplayName = accountEntity.DisplayName,
                EMail = accountEntity.EMail,
                LastLogin = accountEntity.LastLogin,
                UserName = accountEntity.UserName,
            };
        }

    }
}
