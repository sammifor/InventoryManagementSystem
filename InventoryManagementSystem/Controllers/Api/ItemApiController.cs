using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ResultModels;
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
    public class ItemApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public ItemApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /*
         * ItemApi/GetItemsByEquipId/{EquipmentId}
         */
        //查詢設備底下Items
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        [Authorize]
        public async Task<ItemResultModel[]> GetItemsByEquipId(Guid id)
        {
            var results = await _dbContext.Items
                .Where(i => i.EquipmentId == id && i.ConditionId != "D")
                .Select(i => new ItemResultModel
                {
                    ItemId = i.ItemId,
                    ItemSn = i.ItemSn,
                    Condition = i.Condition.ConditionName,
                    Description = i.Description,

                    EquipmentSn = i.Equipment.EquipmentSn,
                    EquipmentName = i.Equipment.EquipmentName,
                    CategoryName=i.Equipment.EquipmentCategory.CategoryName

                })
                .ToArrayAsync();

            return results;
        }

        /*
         * ItemApi/InserItem
         */
        // 新增 Item
        // 若成功新增，return ItemID
        [HttpPost]
        [Consumes("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> InsertItem(InsertItemViewModel model)
        {
            if(string.IsNullOrWhiteSpace(model.ItemSn))
            {
                return BadRequest();
            }

            Guid itemId = Guid.NewGuid();
            Item item = new Item
            {
                ItemId = itemId,
                EquipmentId = model.EquipmentId,
                ItemSn = model.ItemSn,
                Description = model.Description,
                ConditionId = "I",
            };
            _dbContext.Items.Add(item);

            // Get AdminID
            string adminIDString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
            ItemLog log = new ItemLog
            {
                ItemLogId = Guid.NewGuid(),
                AdminId = Guid.Parse(adminIDString),
                ItemId = itemId,
                ConditionId = item.ConditionId,
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

            return Ok(item.ItemId);
        }

        /*
         * ItemApi/EditItem/{ItemID}
         */
        // 更改 item 資訊
        [HttpPut]
        [Consumes("application/json")]
        [Route("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditItem(Guid id, EditItemViewModel model)
        {
            if(id != model.ItemId)
            {
                return BadRequest();
            }

            Item item = await _dbContext.Items.FindAsync(id);

            if(item == null)
            {
                return NotFound();
            }

            item.ItemSn = model.ItemSn;
            item.Description = model.Description;

            //_dbContext.Entry(item).State = EntityState.Modified;

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
         *  ItemApi/RemoveItemsByIds
         */
        // 以 ID 移除 Item，可一次移除多個 Item
        // Return: 刪除的資料筆數
        [HttpDelete]
        [Consumes("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<int>> RemoveItemsByIds(Guid[] ids)
        {

            // 查出所有待刪的 items
            var items = await _dbContext.Items
                .Where(i => ids.Contains(i.ItemId))
                .ToArrayAsync();

            Console.WriteLine($"可刪除 {items.Length} 筆資料");

            if (items.Length == 0)
            {
                // 找不到任何可刪除的 items，無法刪除
                return NotFound(0);
            }


            // 已借出、待領取、已刪除的 item 無法移除
            string[] restrictions = { "O", "P", "D" };
            bool unremovableItemsExist = items
                .Any(i => restrictions.Contains(i.ConditionId));

            if (unremovableItemsExist)
            {
                return BadRequest(0);
            }

            // Get AdminID
            string adminIDString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;

            foreach(Item item in items)
            {
                item.ConditionId = "D";
                item.ItemSn = null;

                // 新增刪除紀綠
                ItemLog log = new ItemLog
                {
                    ItemLogId = Guid.NewGuid(),
                    ItemId = item.ItemId,
                    ConditionId = item.ConditionId,
                    AdminId = Guid.Parse(adminIDString),
                    CreateTime = DateTime.Now
                };
                _dbContext.Add(log);
            }


            int rowsAffected = 0;
            try
            {
                rowsAffected = await _dbContext.SaveChangesAsync();
            }
            catch
            {
                // 資料庫端發生錯誤，刪除失敗
                return Conflict(0);
            }

            // 顯示實際成功刪除的紀錄數

            /* 除以二的原因是：
             * 若刪除 n 筆，將會更新 2n 個 rows
             * 因為一筆 Item 就有一筆 ItemLog
             */
            return Ok(rowsAffected / 2);
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAvailableItemsByEquipId(Guid id)
        {
            ItemBaseResultModel[] items = await _dbContext.Items
                .Where(i => i.EquipmentId == id)
                .Where(i => i.ConditionId == "I")
                .Select(i => new ItemBaseResultModel
                {
                    ItemId = i.ItemId,
                    ItemSn = i.ItemSn,
                    Description = i.Description
                })
                .ToArrayAsync();

            return Ok(items);
        }

        

    }
}
