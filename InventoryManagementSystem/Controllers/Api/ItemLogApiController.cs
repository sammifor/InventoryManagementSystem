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
    public class ItemLogApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public ItemLogApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /*
         * ItemLogApi/GetLogsByItemId/{ItemID}
         */
        // 以 itemID 查詢 itemlog
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<IActionResult> GetLogsByItemId(int id)
        {
            LogResultModel[] logs = await _dbContext.ItemLogs
                .Where(il => il.ItemId == id)
                .Select(il => new LogResultModel
                {
                    ItemLogId = il.ItemLogId,
                    OrderDetailId = il.OrderDetailId,
                    AdminId = il.AdminId,
                    ItemId = il.ItemId,
                    ConditionName = il.Condition.ConditionName,
                    AdminFullName = il.Admin.FullName,
                    Description = il.Description,
                    CreateTime = il.CreateTime
                })
                .ToArrayAsync();

            return Ok(logs);

        }
    }
}
