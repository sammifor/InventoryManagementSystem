using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.LINE;
using InventoryManagementSystem.Models.NotificationModels;
using InventoryManagementSystem.Models.Password;
using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OrderDetailApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;
        private readonly NotificationService notificationService;
        private readonly LineConfig lineConfig;

        public OrderDetailApiController(
            InventoryManagementSystemContext dbContext,
            NotificationService notificationService,
            IOptions<LineConfig> lineConfig)
        {
            _dbContext = dbContext;
            this.notificationService = notificationService;
            this.lineConfig = lineConfig.Value;
        }

        /* method: POST
         * 
         * url: OrderDetailApi/PickupItemsCheck/
         * 
         * input: A json object having the same structrue as PickupItemsCheckViewModel class.
         * 
         * output: 1. 200 Ok if success.
         *         2. 404 Not Found if the OrderDetailID does not exist or the item is not in stock.
         *         3. 409 Conflit if failing to update the database.
         */
        // 管理員確認某筆 OrderDetail 底下的 Item 被領取
        [HttpPost]
        [Consumes("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PickupItemsCheck(PickupItemsCheckViewModel model)
        {
            // 查詢 detail 是否存在且 item 在庫
            OrderDetail detail = await _dbContext.OrderDetails
                .Where(od => od.OrderDetailId == model.OrderDetailId &&
                    od.Item.ConditionId == "P") // 待領取到領取的過程中，分配的物品都在待命狀態
                .FirstOrDefaultAsync();

            if(detail == null)
            {
                return NotFound();
            }

            // item 狀態改為出庫
            // 因為 FK 的關係，item 一定找得到，所以不再檢查是否為 null
            Item item = await _dbContext.Items
                .FindAsync(detail.ItemId);

            item.ConditionId = "O"; // OutStock
            detail.OrderDetailStatusId = "T"; // Taken

            // Get AdminID
            string adminIdString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
            Guid adminId = Guid.Parse(adminIdString);

            ItemLog log = new ItemLog
            {
                ItemLogId = Guid.NewGuid(),
                OrderDetailId = detail.OrderDetailId,
                AdminId = adminId,
                ItemId = item.ItemId,
                ConditionId = item.ConditionId,
                Description = model.Description,
                CreateTime = DateTime.Now
            };

            _dbContext.ItemLogs.Add(log);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            return Ok();
        }

        /* method: POST
         * 
         * url: OrderDetailApi/ReturnItemsCheck/
         * 
         * input: A JSON object with the same structure as ReturnItemsCheckViewModel class.
         * 
         * output: 1. 200 Ok if success.
         *         2. 404 Not Found if the OrderDetailID does not exist or 
         *                             the OrderDetail does not have the record that the item was taken or
         *                             the Item's condition is not OutStock
         *         3. 409 Conflit if failing to update the database.
         */
        // 管理員確認某筆 OrderDetail 底下的 Item 歸還與 Item 狀態
        [HttpPost]
        [Consumes("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ReturnItemsCheck(ReturnItemsCheckViewModel model)
        {
            OrderDetail detail = await _dbContext.OrderDetails
                .Where(od => od.OrderDetailId == model.OrderDetailId &&
                    od.OrderDetailStatusId == "T" && // Taken
                    od.Item.ConditionId == "O") // OutStock
                .FirstOrDefaultAsync();

            if(detail == null)
            {
                return NotFound();
            }

            detail.OrderDetailStatusId = "R"; // Returned

            // detail 若不是空，因 FK，已保證可以找到 item
            // 故不需檢查 item 是否為 null
            Item item = await _dbContext.Items.FindAsync(detail.ItemId);

            if(model.FunctionsNormally)
                item.ConditionId = "I"; // InStock
            else
                item.ConditionId = "F"; // Failure

            // 若東西全部都歸還、報遺失了，直接把這筆 order 歸類為已結束
            var details = _dbContext.OrderDetails
                .Where(od => od.OrderId == detail.OrderId)
                .AsEnumerable();

            bool allReturned = details
                .All(od => od.OrderDetailStatusId != "T");

            if(allReturned)
            {
                Order order = await _dbContext.Orders.FindAsync(detail.OrderId);
                order.OrderStatusId = "E";
                await SendQuestionnaireLink(detail.OrderId);
            }

            // Get AdminID
            string adminIdString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
            Guid adminId = Guid.Parse(adminIdString);

            ItemLog log = new ItemLog
            {
                ItemLogId = Guid.NewGuid(),
                OrderDetailId = detail.OrderDetailId,
                AdminId = adminId,
                ItemId = item.ItemId,
                ConditionId = item.ConditionId,
                Description = model.Description,
                CreateTime = DateTime.Now
            };

            _dbContext.ItemLogs.Add(log);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            return Ok();
        }

        /* method: POST
         * 
         * url: OrderDetailApi/ItemLostCheck/
         * 
         * input: A JSON object with the same structure as ItemLostCheckViewModel class.
         * 
         * output: 1. 200 Ok if success.
         *         2. 404 Not Found if the OrderDetailID does not exist or 
         *                             the OrderDetail does not have the record that the item was taken or
         *                             the Item's condition is not OutStock
         *         3. 409 Conflit if failing to update the database.
         */
        // 管理員無法確認歸還時，將物品標記為遺失
        [HttpPost]
        [Consumes("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ItemLostCheck(ItemLostCheckViewModel model)
        {
            OrderDetail detail = await _dbContext.OrderDetails
                .Where(od => od.OrderDetailId == model.OrderDetailId &&
                    od.OrderDetailStatusId == "T" && // Taken
                    od.Item.ConditionId == "O") // OutStock
                .FirstOrDefaultAsync();

            if(detail == null)
            {
                return NotFound();
            }

            detail.OrderDetailStatusId = "L"; // LOST

            Item item = await _dbContext.Items.FindAsync(detail.ItemId);
            item.ConditionId = "L"; // LOST


            // 若東西全部都歸還、報遺失了，直接把這筆 order 歸類為已結束
            var details = _dbContext.OrderDetails
                .Where(od => od.OrderId == detail.OrderId)
                .AsEnumerable();

            bool allReturned = details
                .All(od => od.OrderDetailStatusId != "T");

            if(allReturned)
            {
                Order order = await _dbContext.Orders.FindAsync(detail.OrderId);
                order.OrderStatusId = "E";
                await SendQuestionnaireLink(detail.OrderId);
            }

            // Get AdminID
            string adminIdString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
            Guid adminId = Guid.Parse(adminIdString);

            ItemLog log = new ItemLog
            {
                ItemLogId = Guid.NewGuid(),
                OrderDetailId = detail.OrderDetailId,
                AdminId = adminId,
                ItemId = item.ItemId,
                ConditionId = item.ConditionId,
                Description = model.Description,
                CreateTime = DateTime.Now
            };

            _dbContext.ItemLogs.Add(log);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            return Ok();
        }

        private async Task SendQuestionnaireLink(Guid orderId)
        {
            byte[] tokenBytes = new byte[128];
            byte[] hashedToken;
            using(RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                do
                {
                    rng.GetBytes(tokenBytes);
                    hashedToken = SHA512.HashData(tokenBytes);

                    bool tokenExists = await _dbContext.QuestionnaireTokens
                        .AnyAsync(qt => qt.HashedToken.SequenceEqual(hashedToken));

                    if(!tokenExists)
                        break;

                } while(true);
            }


            QuestionnaireToken questionnaireToken = new QuestionnaireToken
            {
                OrderId = orderId,
                HashedToken = hashedToken,
                ExpireTime = DateTime.Now.AddDays(2),
            };
            _dbContext.QuestionnaireTokens.Add(questionnaireToken);

            string tokenBase64 = Convert.ToBase64String(tokenBytes);
            string tokenUrlEncoded = HttpUtility.UrlEncode(tokenBase64);

            var rentalInfo = _dbContext.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => new
                {
                    o.User.Email,
                    o.User.FullName,
                    o.User.Username,
                    o.User.LineId,
                    o.Equipment.EquipmentName
                })
                .FirstOrDefault();

            if(rentalInfo == null)
            {
                return;
            }

            string quesUrl = $"{Request.Scheme}://{Request.Host}/questionnaire?token={tokenUrlEncoded}";

            StringBuilder builder = new StringBuilder();
            builder.Append($"@{rentalInfo.Username} 您好：\n");
            builder.Append($"請對於您本次租借「{rentalInfo.EquipmentName}」填寫滿意度調查問卷。\n");

            string subject = "租借滿意度調查";
            // 有綁 LINE 就傳送 LINE
            if(!string.IsNullOrWhiteSpace(rentalInfo.LineId))
            {
                using(HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();
                    request.Method = new HttpMethod("POST");
                    request.RequestUri = new Uri("https://api.line.me/v2/bot/message/push");
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", lineConfig.AccessToken);

                    request.Content = JsonContent.Create(new
                    {
                        to = "U4ba3f21ed2374e0164d0a802ea81991c",
                        messages = new[]
                        {
                        new
                        {
                            type = "flex",
                            altText = subject,
                            contents = new
                            {
                                type = "bubble",
                                body = new
                                {
                                    type = "box",
                                    layout = "vertical",
                                    contents = new[]
                                    {
                                        new
                                        {
                                            type = "text",
                                            text = builder.ToString(),
                                            wrap = true
                                        }
                                    }
                                },
                                footer = new
                                {
                                    type = "box",
                                    layout = "horizontal",
                                    contents = new[]
                                    {
                                        new
                                        {
                                            type = "button",
                                            style = "primary",
                                            action = new
                                            {
                                                type = "uri",
                                                label = "前往問卷調查",
                                                uri = quesUrl
                                            }
                                        }
                                    }
                                }
                            }
                        } }

                    });


                    HttpResponseMessage response = await client.SendAsync(request);
                    //builder.Insert(0, $"【{subject}】\n");
                    //await notificationService.SendLineNotification(rentalInfo.LineId, builder.ToString());
                }
            }
            else
            {
                builder.Append($"問卷連結：{quesUrl} \n");
                builder.Append($"本連結將於 48 小時後失效。");
                builder.Replace("\n", "<br>");
                builder.Append("<br>");
                builder.Append("本信為系統自動發送，請勿直接回覆此郵件。");
                builder.Insert(0, "<p>");
                builder.Append("</p>");
                await notificationService.SendEmailNotification(
                        rentalInfo.FullName,
                        rentalInfo.Email,
                        subject,
                        "html",
                        builder.ToString());
            }
            return;
        }
    }
}

