using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ResultModels;
using Microsoft.AspNetCore.Authorization;
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
    public class EquipCategoryApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public EquipCategoryApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /*
         * EquipCategoryApi/GetCates 
         */
        // 取得所有 EquipCategory 的 id 和 name
        [HttpGet]
        [Produces("application/json")]
        [Authorize]
        public async Task<IActionResult> GetCates()
        {
            var categories = await _dbContext.EquipCategories
                .Where(c => !c.Deleted)
                .Select(c => new EquipCatesRresultModel()
                {
                    EquipCategoryId = c.EquipCategoryId,
                    CategoryName = c.CategoryName
                })
                .ToArrayAsync();
            return Ok(categories);
        }

        /*
         * EquipCategoryApi/InsertCate
         */
        // 新增種類
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> InsertCate(
            [FromQuery(Name = "name")] 
            string categoryName)
        {
            if(string.IsNullOrWhiteSpace(categoryName))
            {
                return BadRequest();
            }

            EquipCategory category = new EquipCategory
            {
                EquipCategoryId = Guid.NewGuid(),
                CategoryName = categoryName
            };
            _dbContext.EquipCategories.Add(category);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            return Ok(category.EquipCategoryId);
        }

        /*
         * EquipcategoryApi/DeleteCate
         */
        // 刪除種類
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCate(Guid id)
        {
            #region 找不到此分類，不能刪
            EquipCategory category = await _dbContext.EquipCategories
                .Where(c => !c.Deleted)
                .Where(c => c.EquipCategoryId == id)
                .FirstOrDefaultAsync();

            if(category == null)
                return NotFound();
            #endregion

            #region 此分類仍有 equip 未刪，不能刪
            bool equipExists = await _dbContext.Equipment
                .Where(e => e.EquipmentCategoryId == category.EquipCategoryId)
                .AnyAsync(e => !e.Deleted);

            if(equipExists)
                return BadRequest();
            #endregion

            category.Deleted = true;
            category.CategoryName = null;

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
