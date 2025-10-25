using FluentValidation;
namespace FinTrack360.Application.Features.Auth.ConfirmEmail;
public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");
    }
}
