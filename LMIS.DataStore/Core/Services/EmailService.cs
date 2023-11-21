using LMIS.DataStore.Core.Services.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMIS.DataStore.Core.Services
{
    public class EmailService: IEmailService
    {
        public IConfiguration _configuration { get; }
            public EmailService(IConfiguration configuration)
            {
                _configuration = configuration;
            }
        public string SendMail(string email, string subject, string HtmlMessage)
        {


            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress(_configuration["MailSettings:SenderName"], _configuration["MailSettings:SenderEmail"]);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(email, email);
            message.To.Add(to);

            message.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = HtmlMessage;
            //bodyBuilder.TextBody = "Hello World!";

            message.Body = bodyBuilder.ToMessageBody();

            MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();

            // Allow SSLv3.0 and all versions of TLS
            // client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;


            client.Connect("smtp.gmail.com", 587, SecureSocketOptions.Auto);
            client.Authenticate(_configuration["MailSettings:SenderEmail"], _configuration["MailSettings:Password"]);

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();

           

            return "Message sent";
        }
    }
}
