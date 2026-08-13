using FluentValidation;
using RealEstate.Application.DTOs;

namespace RealEstate.Application.Identity.Validators;

public class LoginWithOtpDtoValidator : AbstractValidator<LoginWithOtpDto>
{
    public LoginWithOtpDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9]{10,15}$")
            .WithMessage("Invalid phone number format. It must follow E.164 format (e.g., +9639xxxxxxx).");

        RuleFor(x => x.VerificationCode)
            .NotEmpty().WithMessage("Verification code (OTP) is required.")
            .Matches(@"^\d{6}$").WithMessage("Verification code must be exactly 6 digits.");
    }
}