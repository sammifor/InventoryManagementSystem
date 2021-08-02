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
    [Route("api/extrafee")]
    [ApiController]
    public class ExtraFeeApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public ExtraFeeApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* method: POST
         * 
         * url: /api/extrafee
         * 
         * input:
         *      {
         *          "orderDetailId": "...",
         *          "fee": 0,必填
         *          "description": "..."描述罰金非必填
         *      }
         * 
         * output:
         * 
         */
        // 針對某筆 OrderDetail 新增一筆 ExtraFee
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PostExtraFee(PostExtraFeeViewModel model)
        {
            if(model.Fee <= 0)
                return BadRequest("金額不正確");

            bool orderDetailExists = await _dbContext.OrderDetails
                .AnyAsync(od => od.OrderDetailId == model.OrderDetailId);

            if(!orderDetailExists)
                return NotFound("此筆訂單名細不存在");


            Guid extraFeeId = Guid.NewGuid();
            ExtraFee extraFee = new ExtraFee
            {
                ExtraFeeId = extraFeeId,
                OrderDetailId = model.OrderDetailId,
                Fee = model.Fee,
                Description = model.Description
            };
            _dbContext.ExtraFees.Add(extraFee);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict("更新資料庫失敗");
            }

            return Ok();
        }
    }
}
