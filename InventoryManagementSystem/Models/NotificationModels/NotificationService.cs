using InventoryManagementSystem.Models.LINE;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.NotificationModels
{
    public class NotificationService
    {
        private readonly HttpClient httpClient;

        private readonly SmtpClient smtpClient;

        private readonly IConfiguration Configuration;

        public NotificationService(IConfiguration configuration)
        {
            httpClient = new HttpClient();
            smtpClient = new SmtpClient();
            Configuration = configuration;
        }

        /// <summary>
        /// Send an email.
        /// </summary>
        /// <param name="receiverFullName">The full name of the receiver.</param>
        /// <param name="receiverEamil">The receiver's email address.</param>
        /// <param name="subject">The subject of the mail.</param>
        /// <param name="subtype">Either <c>plain</c> or <c>html</c>.</param>
        /// <param name="bodyText">The content of the mail.</param>
        /// <returns></returns>
        public async Task SendEmailNotification(
            string receiverFullName, 
            string receiverEamil, 
            string subject, 
            string subtype, 
            string bodyText)
        {
            NotificationConfig notificationConfig = Configuration.GetSection("Notification").Get<NotificationConfig>();

            if(!smtpClient.IsConnected)
                await smtpClient.ConnectAsync(notificationConfig.Host, notificationConfig.Port, false);
            if(!smtpClient.IsAuthenticated)
                await smtpClient.AuthenticateAsync(notificationConfig.User, notificationConfig.Pass);


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(notificationConfig.Name, notificationConfig.User));
            message.To.Add(new MailboxAddress(receiverFullName, receiverEamil));

            message.Subject = subject;


            message.Body = new TextPart("html")
            {
                Text = bodyText
            };

            await smtpClient.SendAsync(message);
        }

        public async Task SendLineNotification(string lineId, string text)
        {
            LineConfig lineConfig = Configuration.GetSection("LINE").Get<LineConfig>();
            PushMessage pushMessage = new PushMessage
            {
                to = lineId,
                messages = new[]
                {
                            new LineMessage
                            {
                                type = "text",
                                text = text
                            }
                        }
            };

            HttpContent content = JsonContent.Create<PushMessage>(pushMessage);

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = new HttpMethod("POST");
            request.RequestUri = new Uri("https://api.line.me/v2/bot/message/push");
            request.Headers
                .Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", lineConfig.AccessToken);
            request.Content = content;
            await httpClient.SendAsync(request);
        }
    }
}
