using InventoryManagementSystem.Models.EF;
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
    public class OrderDetailApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public OrderDetailApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /*
         * OrderDetailApi/PickupItemsCheck/
         */
        // 管理員確認某筆 OrderDetail 底下的 Item 被領取
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PickupItemsCheck(PickupItemsCheckViewModel model)
        {
            // 查詢 detail 是否存在且 item 在庫
            OrderDetail detail = await _dbContext.OrderDetails
                .Where(od => od.OrderDetailId == model.OrderDetailId &&
                    od.Item.ConditionId == "I") // 待領取到領取的過程中，分配的物品都在庫
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

            ItemLog log = new ItemLog
            {
                OrderDetailId = detail.OrderDetailId,
                AdminId = 1, // TODO authentication
                ItemId = item.ItemId,
                ConditionId = item.ConditionId,
                Description = model.Description,
                CreateTime = DateTime.Now
            };

            _dbContext.ItemLogs.Add(log);

            detail.OrderDetailStatusId = "T"; // Taken

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

        /*
         * OrderDetailApi/ReturnItemsCheck/
         */
        // 管理員確認某筆 OrderDetail 底下的 Item 歸還與 Item 狀態
        [HttpPost]
        [Consumes("application/json")]
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

            // detail 若不是空，已保證可以找到 item
            // 故不需檢查 item 是否為 null
            Item item = await _dbContext.Items.FindAsync(detail.ItemId);

            if(model.FunctionsNormally)
                item.ConditionId = "I"; // InStock
            else
                item.ConditionId = "F"; // Failure

            ItemLog log = new ItemLog
            {
                OrderDetailId = detail.OrderDetailId,
                AdminId = 1, // TODO authentication
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
