using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ResultModels;
using InventoryManagementSystem.Models.ViewModels;
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

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> RespondOrder(RespondOrderViewModel model)
        {
            var order = await _dbContext.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == model.OrderID);

            // 防止同個 item 被分配多次
            int[] itemIDs = model.ItemIDs.Distinct().ToArray();
            

            // 找不到訂單
            if(order == null)
            {
                return BadRequest("找不到訂單");
            }

            Response response = null;
            int adminID = 1; // For testing
            if(model.Reply == "N")
            {
                //response = new Response
                //{
                //    OrderId = model.OrderID,
                //    AdminId = adminID,
                //    Reply = model.Reply
                //};

                return Ok("已拒絕訂單");
            }
            else if(model.Reply == "Y")
            {
                // 訂單寫的數量與實際分配的數量不一致
                if(order.Quantity != itemIDs.Length)
                {
                    return BadRequest("訂單寫的數量與實際分配的數量不一致");
                }

                // 存在有分配的設備非訂單所寫的設備
                bool invalidEquipIdExists = await _dbContext.Items
                    .AsNoTracking()
                    .Where(i => itemIDs.Contains(i.ItemId))
                    .AnyAsync(i => i.EquipmentId != order.EquipmentId);
                if(invalidEquipIdExists)
                {
                    return BadRequest("存在有分配的設備非訂單所寫的設備");
                }

                // 庫存不夠，無法滿足訂單
                int inStockNumber = await _dbContext.Items
                    .AsNoTracking()
                    .Where(i => i.EquipmentId == order.EquipmentId)
                    .CountAsync(i => i.ConditionId == "I");
                if(inStockNumber < order.Quantity)
                {
                    return BadRequest("庫存不夠，無法滿足訂單");
                }




                return Ok("已核可訂單");

            }
            else
            {
                return BadRequest("REPLY 格式不正確");
            }

            //new Response
            //{
            //    AdminId = 1,
            //    OrderId = model.OrderID,
            //    Reply = ""
            //};
        }

        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> FindManyItems(int[] ids)
        {
            var results = await _dbContext.Items
                .Where(i => ids.Contains(i.ItemId))
                .ToArrayAsync();
            if(results == null)
            {
                return BadRequest();
            }

            return Ok(results);
        }
    }
}
