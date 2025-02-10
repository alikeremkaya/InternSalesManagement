

namespace Ekip2.Application.Services.MailServices
{
    public class MailService : IMailService
    {
        public async Task SendMailAsync(MailDTO mailDTO)
        {//ubmsobcwohmdrvcr
            try
            {
                var newMail = new MimeMessage();
                newMail.From.Add(MailboxAddress.Parse("info.hrmanagementsystem@gmail.com"));
                newMail.To.Add(MailboxAddress.Parse(mailDTO.Email));
                newMail.Subject = mailDTO.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = mailDTO.Message;
                newMail.Body = builder.ToMessageBody();
                var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("info.hrmanagementsystem@gmail.com", "ubmsobcwohmdrvcr");
                await smtp.SendAsync(newMail);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Email Gönderilirken Hata Oluştu : {ex.Message}");
            }
        }
    }
}
