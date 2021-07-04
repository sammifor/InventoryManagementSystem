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
    public class EquipApiController : ControllerBase
    {
        private InventoryManagementSystemContext _dbContext;
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
        public async Task<EquipCatesViewModel[]> GetCates()
        {
            var results = await _dbContext.EquipCategories
                .Select(c => new EquipCatesViewModel()
                {
                    EquipCategoryId = c.EquipCategoryId,
                    CategoryName = c.CategoryName
                })
                .ToArrayAsync();
            return results;
        }

        /*
         * EquipApi/GetEquipByCateId/{EquipCategoryId} 
         */
        //設備名稱下拉式選單
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<EquipViewModel[]> GetEquipByCateId(int id)
        {
            var results = await _dbContext.Equipment
                .Where(e => e.EquipmentCategoryId == id)
                .Select(e => new EquipViewModel
                {
                    EquipmentId = e.EquipmentId,
                    EquipmentSn = e.EquipmentSn,
                    EquipmentName = e.EquipmentName,
                    Brand = e.Brand,
                    Model = e.Model,
                    UnitPrice = e.UnitPrice,
                    Description = e.Description,
                    Quantity = e.Items.Count()
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
        public async Task<ItemViewModel[]> GetItemsByEquipId(int id)
        {
            var results = await _dbContext.Items
                .Where(i => i.EquipmentId == id)
                .Select(i => new ItemViewModel
                {
                    ItemId = i.ItemId,
                    ItemSn = i.ItemSn,
                    Condition = i.Condition.ConditionName,
                    Description = i.Description
                })
                .ToArrayAsync();
                
            return results;
        }
    }
}