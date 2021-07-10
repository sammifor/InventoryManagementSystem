﻿using InventoryManagementSystem.Models.EF;
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
    public class EquipCategoryApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public EquipCategoryApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /*
         * EquipApi/GetCates 
         */
        // 取得所有 EquipCategory 的 id 和 name
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetCates()
        {
            var categories = await _dbContext.EquipCategories
                .Select(c => new EquipCatesRresultModel()
                {
                    EquipCategoryId = c.EquipCategoryId,
                    CategoryName = c.CategoryName
                })
                .ToArrayAsync();
            return Ok(categories);
        }

    }
}
