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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetLogsByItemId(Guid id)
        {
            LogResultModel[] logs = await _dbContext.ItemLogs
                .Where(il => il.ItemId == id)
                .Select(il => new LogResultModel
                {
                    ItemLogId = il.ItemLogId,
                    ItemLogSn = il.ItemLogSn,
                    AdminUsername = il.Admin.Username,
                    OrderSn = il.OrderDetail.Order.OrderSn,
                    OrderDetailSn = il.OrderDetail.OrderDetailSn,
                    ItemSn = il.Item.ItemSn,
                    ConditionName = il.Condition.ConditionName,
                    AdminFullName = il.Admin.FullName,
                    Description = il.Description,
                    CreateTime = il.CreateTime.GetValueOrDefault().ToString("g")
                })
                .ToArrayAsync();

            return Ok(logs);

        }
    }
}
