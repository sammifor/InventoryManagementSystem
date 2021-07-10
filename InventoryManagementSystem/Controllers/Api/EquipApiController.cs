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
    public class EquipApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public EquipApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }


        /*
         * EquipApi/GetCates 
         */
        // 設備種類下拉式選單
        [HttpGet]
        [Produces("application/json")]
        public async Task<EquipCatesRresultModel[]> GetCates()
        {
            var results = await _dbContext.EquipCategories
                .Select(c => new EquipCatesRresultModel()
                {
                    EquipCategoryId = c.EquipCategoryId,
                    CategoryName = c.CategoryName
                })
                .ToArrayAsync();
            return results;
        }

        /*
         * EquipApi/GetEquipNamesByCatesId/{EquipCategoryId}
         */
        // 設備名稱下拉式選單（名稱不重覆）
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<string[]> GetEquipNamesByCatesId(int id)
        {
            var results = await _dbContext.Equipment
                .Where(e => e.EquipmentCategoryId == id)
                .Select(e => e.EquipmentName)
                .Distinct()
                .ToArrayAsync();

            return results;
        }

        /*
         * EquipApi/GetEquipById/{EquipmentId}
         */
        // 以 equip ID 查詢 equip 的資訊
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<EquipResultModel>> GetEquipById(int id)
        {
            var result = await _dbContext.Equipment
                .Where(e => e.EquipmentId == id)
                .Select(e => new EquipResultModel
                {
                    EquipmentId = e.EquipmentId,
                    EquipmentSn = e.EquipmentSn,
                    EquipmentName = e.EquipmentName,
                    Brand = e.Brand,
                    Model = e.Model,
                    UnitPrice = e.UnitPrice,
                    Description = e.Description,
                    QuantityUsable = e.Items.Count(i => i.ConditionId == "I" || i.ConditionId == "O"),
                    QuantityInStock = e.Items.Count(i => i.ConditionId == "I")
                })
                .FirstOrDefaultAsync();

            if(result == null)
            {
                return NotFound();
            }


            return result;

        }


        /*
         * EquipApi/GetEquipByEquipName/{EquipmentName}
         */
        // 以設備名稱查詢設備（名稱完全一致）
        [HttpGet]
        [Produces("application/json")]
        [Route("{name}")]
        public async Task<EquipResultModel[]> GetEquipByEquipName(string name)
        {
            var results = await _dbContext.Equipment
                .Where(e => e.EquipmentName == name)
                .Select(e => new EquipResultModel
                {
                    EquipmentId = e.EquipmentId,
                    EquipmentCategoryId = e.EquipmentCategoryId,
                    EquipmentSn = e.EquipmentSn,
                    EquipmentName = e.EquipmentName,
                    Brand = e.Brand,
                    Model = e.Model,
                    UnitPrice = e.UnitPrice,
                    Description = e.Description,
                    QuantityUsable = e.Items.Count(i => i.ConditionId == "I" || i.ConditionId == "O"),
                    QuantityInStock = e.Items.Count(i => i.ConditionId == "I")
                })
                .ToArrayAsync();
            return results;
        }

        /*
         * EquipApi/GetEquipByCateId/{EquipCategoryId} 
         */
        // 以設備種類 ID 查詢該種類下的所有設備
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<EquipResultModel[]> GetEquipByCateId(int id)
        {
            var results = await _dbContext.Equipment
                .Where(e => e.EquipmentCategoryId == id)
                .Select(e => new EquipResultModel
                {
                    EquipmentId = e.EquipmentId,
                    EquipmentCategoryId = e.EquipmentCategoryId,
                    EquipmentSn = e.EquipmentSn,
                    EquipmentName = e.EquipmentName,
                    Brand = e.Brand,
                    Model = e.Model,
                    UnitPrice = e.UnitPrice,
                    Description = e.Description,
                    QuantityUsable = e.Items.Count(i => i.ConditionId == "I" || i.ConditionId == "O"),
                    QuantityInStock = e.Items.Count(i => i.ConditionId == "I")
                })
                .ToArrayAsync();
            return results;
        }

        /*
         * EquipApi/GetItemsByEquipId/{EquipmentId}
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
                    Description = i.Description
                })
                .ToArrayAsync();

            return results;
        }

        /*
         * EquipApi/InsertEquip
         */
        // 新增 Equip
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<InsertEquipResultModel> InsertEquip(Equipment equip)
        {
            // TODO InsertEquipViewModel，防止 overposting
            await _dbContext.Equipment.AddAsync(equip);
            InsertEquipResultModel result = null;
            try
            {
                await _dbContext.SaveChangesAsync();
                result = new InsertEquipResultModel(true);
            }
            catch(DbUpdateException e)
            {
                // 新增失敗
                result = new InsertEquipResultModel(false);
            }

            return result;
        }


        /*
         * EquipApi/EditEquip/{EquipmentId}
         */
        // 編輯一筆 Equip 資料
        [HttpPut]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<IActionResult> EditEquip(int id, Equipment equip)
        {
            if(equip.EquipmentId != id)
            {
                return BadRequest();
            }

            _dbContext.Entry(equip).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateException e)
            {
                return Conflict();
            }
            return NoContent();
        }

        /*
         *  EquipApi/RemoveEquipByIds
         */
        // 以 ID 移除 Equip，可一次移除多個 Equip
        // Return: 刪除的資料筆數
        [HttpDelete]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<int>> RemoveEquipByIds(int[] ids)
        {

            // 查出所有待刪的 equip
            var equipPieces = await _dbContext.Equipment
                .Where(e => ids.Contains(e.EquipmentId))
                .ToArrayAsync();
            Console.WriteLine($"可刪除 {equipPieces.Length} 筆資料");

            if(equipPieces.Length == 0)
            {
                // 找不到任何可刪除的設備，無法刪除
                return NotFound(0);
            }


            // 判斷這些 equip 底下有沒有任何 item
            bool hasItems = await _dbContext.Items
                .AnyAsync(i => ids.Contains(i.EquipmentId));

            if(hasItems)
            {
                // 有 Equip 底下還存在著 Item，無法刪除
                return BadRequest(0);
            }

            _dbContext.Equipment.RemoveRange(equipPieces);
            int rowsAffected = 0;
            try
            {
                rowsAffected = await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException e)
            {
                // 資料庫端發生錯誤，刪除失敗
                return Conflict(0);
            }

            // 顯示實際成功刪除的紀錄數
            return Ok(rowsAffected);
        }
    }
}