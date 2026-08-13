using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;    
using RealEstate.Application.Common.Extensions;
using RealEstate.Domain.PhoneVerifications;
using RealEstate.Infrastructure.Persistence;
using Telegram.Bot.Types;
namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/telegram")]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TelegramWebhookController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleUpdate([FromBody] Update update)
        {
            if (update?.Message == null) return Ok();

            string? rawPhoneNumber = null;

            if (update.Message.Contact != null)
            {
                rawPhoneNumber = update.Message.Contact.PhoneNumber;
            }
            else if (!string.IsNullOrWhiteSpace(update.Message.Text))
            {
                rawPhoneNumber = update.Message.Text.Trim();
            }

            if (string.IsNullOrEmpty(rawPhoneNumber))
            {
                return Ok();
            }

            var canonicalPhone = rawPhoneNumber.ToCanonicalE164();
            var chatId = update.Message.Chat.Id.ToString();

            var verification = await _context.PhoneVerifications
                .FirstOrDefaultAsync(v => v.PhoneNumber == canonicalPhone);

            if (verification != null)
            {
                verification.TelegramChatId = chatId;
                verification.ExpiresAtUtc = DateTime.UtcNow;
            }
            else
            {
                var newVerification = new PhoneVerification(Guid.NewGuid())
                {
                    PhoneNumber = canonicalPhone,
                    TelegramChatId = chatId,
                    CreatedAtUtc = DateTime.UtcNow,
                    ExpiresAtUtc = DateTime.UtcNow,
                    IsUsed = false
                };

                await _context.PhoneVerifications.AddAsync(newVerification);
            }
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
