using FluentValidation;
using RealEstate.Application.Identity.DTOs;

namespace RealEstate.Application.Identity.Validators;

public class RegisterWithOtpDtoValidator : AbstractValidator<RegisterWithOtpDto>
{
    public RegisterWithOtpDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
    .NotEmpty().WithMessage("Phone number is required.")
    .Matches(@"^\+?[0-9]{10,15}$").WithMessage("Phone number format is invalid.");

        RuleFor(x => x.VerificationCode)
    .NotEmpty().WithMessage("Verification code (OTP) is required.")
    .Matches(@"^\d{6}$").WithMessage("Verification code must be exactly 6 digits.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
    }
}