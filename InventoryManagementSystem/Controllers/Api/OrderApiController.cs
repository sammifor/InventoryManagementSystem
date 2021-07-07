using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ResultModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{

    [Route("[controller]/[action]")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {

        private readonly InventoryManagementSystemContext _dbContext;

        public OrderApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }


        /*
         * OrderApi/MakeOrder
         */
        // 下訂單
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<MakeOrderMessageResultModel> MakeOrder(Order order)
        {
            // TODO 建立下訂單的 ViewModel，防止 overposting
            int equipId = 0;
            string cookieKey = "selected-equip";
            Response.Cookies.Delete(cookieKey);
            try
            {
                // 試著在 cookie 找 equipId
                equipId = int.Parse(Request.Cookies[cookieKey]);
            }
            catch(Exception)
            {
                return new MakeOrderMessageResultModel(false);
            }

            order.EquipmentId = equipId;
            await _dbContext.AddAsync(order);

            try
            {
                await _dbContext.SaveChangesAsync();

                // 下訂單成功
                return new MakeOrderMessageResultModel(true);
            }
            catch(DbUpdateException e)
            {
                // 下訂單失敗
                return new MakeOrderMessageResultModel(false);
            }

        }

        /*
         * OrderApi/GetOrdersByUserId/{UserID}
         */
        // 以 UserID 查詢所有訂單（任何狀態的訂單都會查出來）
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<OrderResultModel[]> GetOrdersByUserId(int id)
        {
            var results = await _dbContext.Orders
                .Where(o => o.UserId == id)
                .Select(o => new OrderResultModel
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    EquipmentId = o.EquipmentId,
                    Quantity = o.Quantity,
                    EstimatedPickupTime = o.EstimatedPickupTime,
                    Day = o.Day,
                    OrderStatusId = o.OrderStatusId,
                    OrderTime = o.OrderTime,

                    EquipmentSn = o.Equipment.EquipmentSn,
                    EquipmentName = o.Equipment.EquipmentName,
                    Brand = o.Equipment.Brand,
                    Model = o.Equipment.Model,
                    UnitPrice = o.Equipment.UnitPrice,
                    Description = o.Equipment.Description,

                    Username = o.User.Username,

                    StatusName = o.OrderStatus.StatusName
                })
                .ToArrayAsync();

            return results;
        }
    }
}
