using FluentValidation;
using MessagingService.Api.Helpers;
using System.Collections.Generic;

namespace MessagingService.Api.Models.Auth
{
    public class RegisterRequest
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }

        public KeyValuePair<bool, string> IsValid() => new RegisterRequestValidator().IsValid(this);
    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.DisplayName).MinimumLength(4);
            RuleFor(x => x.EMail).EmailAddress();
            RuleFor(x => x.UserName).MinimumLength(4).MaximumLength(12);
            RuleFor(x => x.Password).
                MinimumLength(6).
                Matches("[a-z]").WithMessage("Password must contain a lowercase letter").
                Matches("[A-Z]").WithMessage("Password must contain a uppercase letter").
                Matches("[0-9]").WithMessage("Password must contain a digit");

        }
    }

}
