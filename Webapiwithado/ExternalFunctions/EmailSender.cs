using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Webapiwithado.Interface;
using System.Net.Mail;

namespace Webapiwithado.ExternalFunctions
{
    public class EmailSender : IEmailSender


    {

        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {


            var mail = _configuration["EmailSettings:Username"];
            var pw = _configuration["EmailSettings:Password"];

            


            var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                EnableSsl = true,
                Port = 587,
                Credentials = new System.Net.NetworkCredential(mail, pw)
            };

            return smtpClient.SendMailAsync(
                new MailMessage(
                    from: mail,
                    to: email,
                    subject: subject,
                    body: message
                    
                
                ));
            

        }
    }
}
