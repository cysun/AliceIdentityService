using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AliceIdentityService.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string senderName;
        private readonly string senderEmail;
        private readonly string apiKey;

        public EmailSender(IConfiguration configuration)
        {
            senderName = configuration["SendGrid:SenderName"];
            senderEmail = configuration["SendGrid:SenderEmail"];
            apiKey = configuration["SendGrid:ApiKey"];
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(senderEmail, senderName),
                Subject = subject,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email));
            msg.SetClickTracking(false, false);
            return client.SendEmailAsync(msg);
        }
    }
}
