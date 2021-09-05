using FluentValidation;
using MessagingService.Api.Helpers;
using System.Collections.Generic;

namespace MessagingService.Api.Models.Message
{
    public class BlockUserRequest
    {
        public string OpponentUser { get; set; }
        public KeyValuePair<bool, string> IsValid() => new BlockUserRequestValidator().IsValid(this);
    }
    public class BlockUserRequestValidator : AbstractValidator<BlockUserRequest>
    {
        public BlockUserRequestValidator()
        {
            RuleFor(x => x.OpponentUser).MinimumLength(4).MaximumLength(12);
        }
    }
}
