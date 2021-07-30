using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.LINE;
using InventoryManagementSystem.Models.NotificationModels;
using InventoryManagementSystem.Models.ResultModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;
        private readonly NotificationConfig _notificaionConfig;
        private readonly LineConfig _lineConfig;
        private static HttpClient Client = new HttpClient();

        public NotificationApiController(
            InventoryManagementSystemContext dbContext, 
            IOptions<NotificationConfig> notificationConfig, 
            IOptions<LineConfig> lineConfig)
        {
            _dbContext = dbContext;
            _notificaionConfig = notificationConfig.Value;
            _lineConfig = lineConfig.Value;
        }

        [HttpPost]
        public async Task<IActionResult> PostNotification()
        {
            string authString = HttpContext.Request.Headers["Authorization"];

            if(authString == null || !authString.StartsWith("basic"))
                return Unauthorized();

            string authKey = authString.Substring("basic ".Length).Trim();

            if(authKey != _notificaionConfig.ApiKey)
                return Unauthorized();

            var usersWithOverdueOrder = await _dbContext.Users
                .Where(u => u.Orders.Any(o => o.OrderStatusId == "A"))
                .Where(u => u.Orders.Any(o => o.OrderDetails.Any(od => od.OrderDetailStatusId == "T")))
                .Where(u => u.Orders.Any(o => o.EstimatedPickupTime.AddDays(o.Day) < DateTime.Now))
                .Where(u => u.AllowNotification == true)
                .Select(u => new
                {
                    FullName = u.FullName,
                    Username = u.Username,
                    Email = u.Email,
                    LineId = u.LineId,
                    OverdueOrders = u.Orders
                        .Where(o => o.OrderStatusId == "A")
                        .Where(o => o.OrderDetails.Any(od => od.OrderDetailStatusId == "T"))
                        .Where(o => o.EstimatedPickupTime.AddDays(o.Day) < DateTime.Now)
                        .Select(o => new
                        {
                            EquipmentName = o.Equipment.EquipmentName,
                            Number = o.OrderDetails.Count(od => od.OrderDetailStatusId == "T")
                        })
                })
                .ToArrayAsync();

            using(var smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_notificaionConfig.Host, _notificaionConfig.Port, false);
                await smtpClient.AuthenticateAsync(_notificaionConfig.User, _notificaionConfig.Pass);

                foreach(var user in usersWithOverdueOrder)
                {
                    #region Build text message
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat("@{0} 您好：\n\n", user.Username);
                    foreach(var order in user.OverdueOrders)
                    {
                        builder.AppendFormat("您所租借的「{0}」仍有 {1} 筆尚未歸還。\n", order.EquipmentName, order.Number);
                    }
                    builder.Append("\n");
                    builder.Append("請您儘速歸還，感謝您的配合。");

                    string lineText = builder.ToString();

                    builder.Append("<br>");
                    builder.Replace("\n", "<br>");
                    builder.Append("本信為系統自動發送，請勿直接回覆此信件。");
                    builder.Insert(0, "<p>");
                    builder.Append("</p>");
                    string emailText = builder.ToString();
                    #endregion

                    #region Send Email
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(_notificaionConfig.Name, _notificaionConfig.User));
                    message.To.Add(new MailboxAddress(user.FullName, user.Email));

                    message.Subject = "逾期通知";


                    message.Body = new TextPart("html")
                    {
                        Text = emailText
                    };

                    await smtpClient.SendAsync(message);
                    #endregion

                    #region Send Line Message
                    // 有綁定 LINE 才傳送訊息
                    if(string.IsNullOrWhiteSpace(user.LineId))
                        continue;

                    PushMessage pushMessage = new PushMessage
                    {
                        to = user.LineId,
                        messages = new[]
                        {
                            new LineMessage
                            {
                                type = "text",
                                text = lineText
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
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _lineConfig.AccessToken);
                    request.Content = content;
                    await Client.SendAsync(request);

                    #endregion
                }

                if(smtpClient.IsConnected)
                    await smtpClient.DisconnectAsync(true);
            }
            return Ok();
        }
    }
}
