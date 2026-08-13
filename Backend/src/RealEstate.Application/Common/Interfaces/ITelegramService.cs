namespace RealEstate.Application.Common.Interfaces
{
    public interface ITelegramService
    {
        Task<bool> SendOtpAsync(string chatId, string otpCode);
    }
}
