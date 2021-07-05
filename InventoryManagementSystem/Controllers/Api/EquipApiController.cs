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
        [HttpDelete]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<RemoveEquipResultModel[]> RemoveEquipByIds(int[] ids)
        {
            var results = new RemoveEquipResultModel[ids.Length];
            for(int i = 0; i < ids.Length; i++)
            {
                var equip = await _dbContext.Equipment
                    .FindAsync(ids[i]);

                if(equip == null)
                {
                    // 找不到設備，無法刪除
                    throw new NotImplementedException();
                }

                if(equip.Items.Any(i => i.ConditionId == "O"))
                {
                    // 底下有 Item 已出庫，無法刪除
                    throw new NotImplementedException();
                }

                _dbContext.Equipment.Remove(equip);
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException e)
                {
                    // 資料庫端發生錯誤，刪除失敗
                    throw new NotImplementedException();
                }
            }
            return results;
        }
    }
}