using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.NotificationModels;
using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("api/extrafee")]
    [ApiController]
    public class ExtraFeeApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;
        private readonly NotificationService notificationService;

        public ExtraFeeApiController(InventoryManagementSystemContext dbContext, NotificationService notificationService)
        {
            _dbContext = dbContext;
            this.notificationService = notificationService;
        }

        /* method: POST
         * 
         * url: /api/extrafee
         * 
         * input:
         *      {
         *          "orderDetailId": "...",
         *          "fee": 0,必填
         *          "description": "..."描述罰金非必填
         *      }
         * 
         * output:
         * 
         */
        // 針對某筆 OrderDetail 新增一筆 ExtraFee
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PostExtraFee(PostExtraFeeViewModel model)
        {
            if(model.Fee <= 0)
                return BadRequest("金額不正確");

            bool orderDetailExists = await _dbContext.OrderDetails
                .AnyAsync(od => od.OrderDetailId == model.OrderDetailId);

            if(!orderDetailExists)
                return NotFound("此筆訂單名細不存在");


            Guid extraFeeId = Guid.NewGuid();
            ExtraFee extraFee = new ExtraFee
            {
                ExtraFeeId = extraFeeId,
                OrderDetailId = model.OrderDetailId,
                Fee = model.Fee,
                Description = model.Description
            };
            _dbContext.ExtraFees.Add(extraFee);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict("更新資料庫失敗");
            }

            var extraFeeInfo = await _dbContext.OrderDetails
                .Where(od => od.OrderDetailId == model.OrderDetailId)
                .Select(od => new
                {
                    EquipmentName = od.Order.Equipment.EquipmentName,
                    User = new
                    {
                        FullName = od.Order.User.FullName,
                        UserId = od.Order.UserId,
                        LineId = od.Order.User.LineId,
                        Email = od.Order.User.Email,
                        Username = od.Order.User.Username
                    }
                })
                .FirstOrDefaultAsync();

            StringBuilder builder = new StringBuilder();
            builder.Append($"@{extraFeeInfo.User.Username} 您好：\n");
            builder.Append($"您所租借的「{extraFeeInfo.EquipmentName}」產生額外費用新台幣 {model.Fee} 元整。請儘速前往付款。若有任何疑問請聯絡我們，謝謝您的配合。");
            string lineText = builder.ToString();
            await notificationService.SendLineNotification(extraFeeInfo.User.LineId, lineText, extraFeeInfo.User.UserId);

            builder.Replace("\n", "<br>");
            builder.Insert(0, "<p>");
            builder.Append("</p>");
            string emailText = builder.ToString();
            await notificationService.SendEmailNotification(extraFeeInfo.User.FullName, extraFeeInfo.User.Email, "額外費用通知", "html", emailText);



            return Ok();
        }
    }
}
