using Inv.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inv.Controllers.Api
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

        //設備種類下拉式選單
        [HttpGet]
        public IActionResult cates()
        {
            var result = _dbContext.EquipCategories.Where(c => c.CategoryName != null).Select(c => new { 
                CategoryName = c.CategoryName,
                EquipCategoryId = c.EquipCategoryId
            }).ToList();
            return this.Ok(result);
        }

        //設備名稱下拉式選單
        [HttpGet]
        public IActionResult equips()
        {
            var result = _dbContext.Equipment.Where(e => e.EquipmentName != null).Select(e =>new {
                EquipmentName = e.EquipmentName,
                EquipmentCategoryId = e.EquipmentCategoryId
            }).Distinct().ToList();
            return this.Ok(result);
        }

        //查詢設備結果(IAN)
        [HttpGet]
        [Route("/equipsqry/equips/{ename}")]
        public IActionResult equipsQryName(String ename)
        {
            var result = _dbContext.Equipment.Where(e => e.EquipmentName == ename).ToList();
            if(result.Count<=0)
            {
                return BadRequest($"設備名稱{ename} 查無資料");
            }
            return this.Ok(result);
        }

        //設備Equipment底下item數量
        [HttpGet]
        public IActionResult itemsQryEquip(Models.Equipment equipment)
        {
            var result = _dbContext.Items.Select(i => i.EquipmentId == equipment.EquipmentId).Count();
            return this.Ok(result);
        }

    }
}
