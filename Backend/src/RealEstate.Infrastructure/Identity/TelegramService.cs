using RealEstate.Application.Common.Interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
namespace RealEstate.Infrastructure.Identity
{
    public class TelegramService : ITelegramService
    {
        private readonly HttpClient _httpClient;
        private readonly string _botToken;
        public TelegramService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _botToken = configuration["TelegramSettings:BotToken"] ?? throw new ArgumentNullException("TelegramBotToken is not configured.");
        }
        public async Task<bool> SendOtpAsync(string chatId, string otpCode)
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";

            var payload = new
            {
                chat_id = chatId,
                text = $"Your verification code is: *{otpCode}*\n\nThis code is valid for 5 minutes.",
                parse_mode = "Markdown"
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            return response.IsSuccessStatusCode;
        }


    }
}
