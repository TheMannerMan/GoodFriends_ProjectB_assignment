using System;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MimeKit.IO;

namespace Services
{
    public class EmailService : IEmailSender
    {
        private readonly IHostEnvironment _environment;
        public EmailService(IHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var pickupDirectory = Path.Combine(_environment.ContentRootPath, "TempMail");
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("test@test.com"));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };

            await SaveToPickupDirectory(message, pickupDirectory);
            //await SendEmail(message);
            await Task.CompletedTask;
        }

        private async Task SaveToPickupDirectory(MimeMessage message, string pickupDirectory)
        {
            if (!Directory.Exists(pickupDirectory))
            {
                Directory.CreateDirectory(pickupDirectory);
            }
            var path = Path.Combine(pickupDirectory, Guid.NewGuid().ToString() + ".eml");
            using (var stream = File.Create(path))
            {
                await message.WriteToAsync(stream);
            }
        }

        public async Task SendEmail(MimeMessage message)
        {
            message.From.Add(new MailboxAddress("Sender Name", "fromemail"));

            using (var client = new SmtpClient())
            {
                // For demo purposes, accept all SSL certificates
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                // Connect to the SMTP server
                client.Connect("smtp.example.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Authenticate using your SMTP username and password
                client.Authenticate("smtp_username", "smtp_password");

                // Send the email
                await client.SendAsync(message);

                // Disconnect from the SMTP server
                client.Disconnect(true);
            }
        }
    }
}
