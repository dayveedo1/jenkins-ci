using EazyMobile.DAL.Data.Config;
using EazyMobile.DAL.Data.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.Repos
{
    public class SendGridEmailService : ISendGridEmailService

    {
        //private IConfiguration configuration;
        private readonly SendGridEmailServiceConfig configuration;

        public SendGridEmailService(SendGridEmailServiceConfig configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            //var apiKey = configuration["SendGridAPIKey"];
            var apiKey = configuration.ApiKey;
            var client = new SendGridClient(apiKey);

            //var from = new EmailAddress("anumbe.terseer@datalinksltd.com", "Terseer Anumbe");
            var from = new EmailAddress(configuration.SenderEmail, configuration.SenderName);

            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);

        }
    }
}
