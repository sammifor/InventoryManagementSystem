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
        //設備種類下拉式選單
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

        //查詢設備結果(IAN)
        [HttpGet]
        [Route("/equipsqry/equips/{ename}")]
        public IActionResult equipsQryName(String ename)
        {
            var result = _dbContext.Equipment.Where(e => e.EquipmentName == ename).ToList();
            if(result.Count <= 0)
            {
                return BadRequest($"設備名稱{ename} 查無資料");
            }
            return this.Ok(result);
        }


    }
}
