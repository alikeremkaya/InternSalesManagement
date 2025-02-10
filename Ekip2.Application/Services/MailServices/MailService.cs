

namespace Ekip2.Application.Services.MailServices
{
    public class MailService : IMailService
    {
        public async Task SendMailAsync(MailDTO mailDTO)
        {//ubmsobcwohmdrvcr
            try
            {
                var newMail = new MimeMessage();
                newMail.From.Add(MailboxAddress.Parse("rty"));
                newMail.To.Add(MailboxAddress.Parse(mailDTO.Email));
                newMail.Subject = mailDTO.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = mailDTO.Message;
                newMail.Body = builder.ToMessageBody();
                var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("hgf", "ytr");
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
