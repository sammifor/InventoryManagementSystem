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
        public async Task<ItemResultModel[]> GetItemsByEquipId(int id)
        {
            var results = await _dbContext.Items
                .Where(i => i.EquipmentId == id)
                .Select(i => new ItemResultModel
                {
                    ItemId = i.ItemId,
                    ItemSn = i.ItemSn,
                    Condition = i.Condition.ConditionName,
                    Description = i.Description,

                    EquipmentSn = i.Equipment.EquipmentSn,
                    EquipmentName = i.Equipment.EquipmentName
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
        public async Task<IActionResult> InsertItem(InsertItemViewModel model)
        {
            Item item = new Item
            {
                EquipmentId = model.EquipmentId,
                ItemSn = model.ItemSn,
                Description = model.Description,
                ConditionId = "I",
            };

            try
            {
                _dbContext.Items.Add(item);
                await _dbContext.SaveChangesAsync();

            }
            catch
            {
                return Conflict();
            }

            ItemLog log = new ItemLog
            {
                AdminId = model.AdminId,
                ItemId = item.ItemId,
                ConditionId = item.ConditionId,
                CreateTime = DateTime.Now
            };

            try
            {
                _dbContext.ItemLogs.Add(log);
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
        public async Task<IActionResult> EditItem(int id, EditItemViewModel model)
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

            _dbContext.Entry(item).State = EntityState.Modified;

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
