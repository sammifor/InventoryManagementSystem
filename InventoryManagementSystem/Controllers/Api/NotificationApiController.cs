using InventoryManagementSystem.Models.EF;
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
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;
        private readonly NotificationConfig _config;

        public NotificationApiController(InventoryManagementSystemContext dbContext, IOptions<NotificationConfig> config)
        {
            _dbContext = dbContext;
            _config = config.Value;
        }

        [HttpPost]
        public async Task<IActionResult> PostNotification()
        {
            string authString = HttpContext.Request.Headers["Authorization"];

            if(authString == null || !authString.StartsWith("basic"))
                return Unauthorized();

            string authKey = authString.Substring("basic ".Length).Trim();

            if(authKey != _config.ApiKey)
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




            using(var client = new SmtpClient())
            {
                await client.ConnectAsync(_config.Host, _config.Port, false);
                await client.AuthenticateAsync(_config.User, _config.Pass);

                foreach(var user in usersWithOverdueOrder)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(_config.Name, _config.User));
                    message.To.Add(new MailboxAddress(user.FullName, user.Email));

                    message.Subject = "逾期通知";

                    StringBuilder builder = new StringBuilder();
                    builder.Append("<p>");
                    builder.AppendFormat("@{0} 您好：<br><br>", user.Username);
                    foreach(var order in user.OverdueOrders)
                    {
                        builder.AppendFormat("您所租借的「{0}」仍有 {1} 筆尚未歸還。<br>", order.EquipmentName, order.Number);
                    }
                    builder.Append("<br>");
                    builder.Append("請您儘速歸還，感謝您的配合。<br>");
                    builder.Append("本信為系統自動發送，請勿直接回覆此郵件。");
                    builder.Append("</p>");

                    message.Body = new TextPart("html")
                    {
                        Text = builder.ToString()
                    };

                    await client.SendAsync(message);
                }

                if(client.IsConnected)
                    await client.DisconnectAsync(true);
            }
            return Ok();
        }
    }
}
