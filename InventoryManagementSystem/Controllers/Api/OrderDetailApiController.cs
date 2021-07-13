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
         * OrderDetailApi/PickupItemsCheck/{OrderDetailID}
         */
        // 管理員確認某筆 OrderDetail 底下的 Item 被領取
        [HttpPost]
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
    }
}
