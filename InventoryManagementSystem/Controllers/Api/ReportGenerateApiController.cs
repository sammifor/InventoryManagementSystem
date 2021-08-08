using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.NotificationModels;
using InventoryManagementSystem.Models.ResultModels;
using InventoryManagementSystem.Models.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReportGenerateApiController : ControllerBase
    {

        private readonly InventoryManagementSystemContext _dbContext;


        public ReportGenerateApiController(
            InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;

        }

        [HttpGet]
        [Route("{StartDate}/{EndDate}")]
        [Produces("application/json")]
        // 查詢order報表
        public async Task<IActionResult> GetOrderReport(DateTime StartDate, DateTime EndDate)
        {
            if (StartDate > EndDate) {
                return BadRequest();
            
            }
            IQueryable<Order> tempOrders = null;
            tempOrders = _dbContext.Orders.Select(o => o);

            OrderReportResultModel[] orders = await tempOrders.Select(o => new OrderReportResultModel
            {
                OrderTime = o.OrderTime,
                OrderId = o.OrderId,
                OrderSn = o.OrderSn,
                UserId = o.UserId,
                Username = o.User.Username,
                EquipmentId = o.EquipmentId,
                EquipmentSn = o.Equipment.EquipmentSn,
                EquipmentName = o.Equipment.EquipmentName,
                Brand = o.Equipment.Brand,
                Model = o.Equipment.Model,
                Quantity = o.Quantity,
                EstimatedPickupTime = o.EstimatedPickupTime,
                OrderStatusId = o.OrderStatusId,
                OrderDetails = o.OrderDetails.Select(od => new OrderDetailResultModel
                {
                    ItemId = od.ItemId,
                    ItemSn = od.Item.ItemSn,
                    ItemStatus = od.ItemLogs
                        .OrderByDescending(il => il.CreateTime)
                        .Select(il => il.Condition.ConditionName)
                        .FirstOrDefault(),
                    Fee=od.ExtraFees
                        .Select(f => f.Fee)
                        .ToArray()
                        .Sum()

                })
                .ToArray(),

                TotalRentalFee = o.Equipment.UnitPrice * o.Quantity * o.Day,

                TotalExtraFee = o.OrderDetails
                            .SelectMany(od => od.ExtraFees)
                            .Select(f => f.Fee)
                            .ToArray()
                            .Sum()
            })
            .Where(o => o.OrderTime >= StartDate && o.OrderTime <= EndDate)
            .Where(o => o.OrderStatusId == "E")
            .ToArrayAsync();

          
            return Ok(orders);

        }


    }
}
