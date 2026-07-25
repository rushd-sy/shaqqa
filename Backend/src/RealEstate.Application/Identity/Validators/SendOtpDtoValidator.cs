using FluentValidation;
using RealEstate.Application.Identity.DTOs;

namespace RealEstate.Application.Identity.Validators;

public class SendOtpDtoValidator : AbstractValidator<SendOtpDto>
{
    public SendOtpDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^[0-9]{10,15}$").WithMessage("Phone number must contain numbers only and be between 10 and 15 digits.");
    }
}