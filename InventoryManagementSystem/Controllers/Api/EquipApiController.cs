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
    public class EquipApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public EquipApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
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

            if (result == null)
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
         * EquipApi/InsertEquip
         */
        // 新增 Equip
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> InsertEquip(InsertEquipViewModel model)
        {
            if(string.IsNullOrWhiteSpace(model.EquipmentSn) || string.IsNullOrWhiteSpace(model.EquipmentName))
            {
                return BadRequest();
            }

            Equipment equip = new Equipment
            {
                EquipmentCategoryId = model.EquipmentCategoryId,
                EquipmentSn = model.EquipmentSn,
                EquipmentName = model.EquipmentName,
                Brand = model.Brand,
                Model = model.Model,
                UnitPrice = model.UnitPrice,
                Description = model.Description
            };

            try
            {
                _dbContext.Equipment.Add(equip);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            return Ok();
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
            if (equip.EquipmentId != id)
            {
                return BadRequest();
            }

            _dbContext.Entry(equip).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
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
        [Consumes("application/json")]
        public async Task<ActionResult<int>> RemoveEquipByIds(int[] ids)
        {

            // 查出所有待刪的 equip
            var equipPieces = await _dbContext.Equipment
                .Where(e => ids.Contains(e.EquipmentId))
                .ToArrayAsync();
            Console.WriteLine($"可刪除 {equipPieces.Length} 筆資料");

            if (equipPieces.Length == 0)
            {
                // 找不到任何可刪除的設備，無法刪除
                return NotFound(0);
            }


            // 判斷這些 equip 底下有沒有任何 item
            bool hasItems = await _dbContext.Items
                .AnyAsync(i => ids.Contains(i.EquipmentId));

            if (hasItems)
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
            catch (DbUpdateConcurrencyException e)
            {
                // 資料庫端發生錯誤，刪除失敗
                return Conflict(0);
            }

            // 顯示實際成功刪除的紀錄數
            return Ok(rowsAffected);
        }

        //關鍵字設備資料
        [HttpGet]
        public IActionResult EquipmentAll()
        {
            var result = _dbContext.Equipment.Where(e => e.EquipmentName != null).Select(e => new {
            
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
            }).ToList();
            return Ok(result);
        }


        /*
         * EquipApi/GetEquipByCateOrName/{categoryId}/{name?}
         */
        // 下拉式選單不強制選取名稱，選種類也可以
        [HttpGet]
        [Produces("application/json")]
        [Route("{categoryId}/{name?}")]
        public async Task<EquipResultModel[]> GetEquipByCateOrName(int categoryId, string name = null)
        {
            var results = _dbContext.Equipment.Where(e => e.EquipmentCategoryId == categoryId);
            if (!string.IsNullOrEmpty(name))
            {
                results = results.Where(e => e.EquipmentName == name);
            }

            return await results.Select(e => new EquipResultModel
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

        }


    }
}