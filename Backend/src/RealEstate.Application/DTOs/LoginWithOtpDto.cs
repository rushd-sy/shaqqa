namespace RealEstate.Application.DTOs
{
    public record LoginWithOtpDto(string PhoneNumber, string VerificationCode);
}
