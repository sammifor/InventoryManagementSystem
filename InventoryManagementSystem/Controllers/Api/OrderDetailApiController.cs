using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OrderDetailApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public OrderDetailApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
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
                .Where(od => od.OrderDetailId == model.OrderDetailID &&
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
    }
}
