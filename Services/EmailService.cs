using MailKit.Net.Smtp;
using MimeKit;

namespace ToDoList.Services
{
    public class EmailService
    {
        private readonly string _email;
        private readonly string _password;
        private readonly ILogger _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _email = configuration["AppEmail"] ?? throw new Exception("Can't get email from config.");
            _password = configuration["AppEmailPassword"] ?? throw new Exception("Can't get password from config.");
            _logger = logger;
        }

        public async Task SendEmail(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ToDoApp", "a222.a2@yandex.ru"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("plain") // или "plain" для обычного текста
            {
                Text = body
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.yandex.ru", 465, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_email, _password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
