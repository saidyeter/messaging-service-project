using FluentValidation;
using MessagingService.Api.Helpers;
using System.Collections.Generic;

namespace MessagingService.Api.Models.Auth
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public KeyValuePair<bool, string> IsValid() => new LoginRequestValidator().IsValid(this);
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName).MinimumLength(4).MaximumLength(12);
            RuleFor(x => x.Password).
                MinimumLength(6).
                Matches("[a-z]").WithMessage("Password must contain a lowercase letter").
                Matches("[A-Z]").WithMessage("Password must contain a uppercase letter").
                Matches("[0-9]").WithMessage("Password must contain a digit");

        }
    }
}
