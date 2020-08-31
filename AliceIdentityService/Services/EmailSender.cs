using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AliceIdentityService.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Scriban;

namespace AliceIdentityService.Services
{
    public class EmailSettings
    {
        public string AppUrl { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool RequireAuthentication { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
    }

    public class EmailSender
    {
        private readonly EmailSettings _settings;
        private readonly string _templateFolder;

        private ILogger<EmailSender> _logger;

        public EmailSender(IWebHostEnvironment env, IOptions<EmailSettings> settings, ILogger<EmailSender> logger)
        {
            _templateFolder = $"{env.ContentRootPath}/EmailTemplates";
            _settings = settings.Value;
            _logger = logger;
        }

        public MimeMessage CreateEmailVerificationMessage(User user, string verificationLink)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            msg.To.Add(new MailboxAddress(user.Name, user.Email));
            msg.Subject = "Alice Identity Service - Email Verification";

            var template = Template.Parse(File.ReadAllText($"{_templateFolder}/EmailVerification.Body.txt"));
            msg.Body = new TextPart("html")
            {
                Text = template.Render(new { link = $"{_settings.AppUrl}{verificationLink}" })
            };

            _logger.LogInformation("Email verification message created for user {user}", user.UserName);

            return msg;
        }

        public async Task SendAsync(MimeMessage msg)
        {
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None);
                if (_settings.RequireAuthentication)
                    await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(msg);
                await client.DisconnectAsync(true);
            }
        }
    }
}
