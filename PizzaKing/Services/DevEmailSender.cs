using System.Text;
using Microsoft.Extensions.Logging;

namespace PizzaKing.Services
{
    public sealed class DevEmailSender : IEmailSender
    {
        private readonly ILogger<DevEmailSender> _logger;
        private readonly IWebHostEnvironment _env;

        public DevEmailSender(ILogger<DevEmailSender> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async Task SendAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("DEV mail TO: {Email}\nSUBJ: {Subj}\n{Body}", email, subject, htmlMessage);

            var dir = Path.Combine(_env.WebRootPath ?? "wwwroot", "_mail");
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, $"{DateTime.Now:yyyyMMdd_HHmmss_fff}_{email}_{San(subject)}.html");
            await File.WriteAllTextAsync(file, htmlMessage, Encoding.UTF8);
        }

        private static string San(string s)
        {
            foreach (var ch in Path.GetInvalidFileNameChars()) s = s.Replace(ch, '_');
            return s.Length > 64 ? s[..64] : s;
        }
    }
}
