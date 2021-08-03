using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ResultModels;
using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public ReportApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        //[HttpGet]
        //[Produces("application/json")]
        //[Route("{id}")]
        //[Authorize]
        //public async Task<IActionResult> GetReportsByOrderDetailId(Guid id)
        //{
        //    GetReportsResultModel[] reports = await _dbContext.Reports
        //        .Where(r => r.OrderDetailId == id)
        //        .Select(r => new GetReportsResultModel
        //        {
        //            ReportId = r.ReportId,
        //            OrderDetailId = r.OrderDetailId,
        //            Description = r.Description,
        //            ReportTime = r.ReportTime,
        //            CloseTime = r.CloseTime
        //        })
        //        .ToArrayAsync();

        //    return Ok(reports);
        //}

        /*
         * ReportApi/GetReports/{id?}
         */
        // 管理員瀏覽所有報修訂單或針對一筆去做瀏覽
        [HttpGet]
        [Produces("application/json")]
        [Route("{id?}")] //OrderDetailId
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetReports(Guid id)
        {
            var reports = _dbContext.Reports.Where(r => r.ReportTime != null);
            if (id != Guid.Empty)
            {
                reports = reports.Where(r => r.OrderDetailId == id);
            }
            var results = await reports.Select(r => new GetReportsResultModel
            {
                ReportId = r.ReportId,
                ReportSn = r.ReportSn,
                OrderSn = r.OrderDetail.Order.OrderSn,
                OrderDetailId = r.OrderDetailId,
                OrderDetailIdSn = r.OrderDetail.OrderDetailSn,
                ItemSn = r.OrderDetail.Item.ItemSn,
                Description = r.Description,
                Note = r.Note,
                ReportTime = r.ReportTime,
                CloseTime = r.CloseTime
            })
                .ToArrayAsync();

            return Ok(results);
        }

        /*
         * ReportApi/PostReport
         */
        // 使用者對某筆 order detail 的新增 report
        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> PostReport(PostReportViewModel model)
        {
            OrderDetail od = await _dbContext.OrderDetails
                .FindAsync(model.OrderDetailId);

            // 不可對不存在的 order detail 新增 report
            if (od == null)
            {
                return NotFound();
            }

            // 有 od 保證一定有 o，故不再檢查 o 是否為 null。
            Order o = await _dbContext.Orders.FindAsync(od.OrderId);

            // Get UserID
            string userIdString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
            Guid userId = Guid.Parse(userIdString);

            // 確認反映問題的 user 是下訂這筆 order 的 user
            if(userId != o.UserId)
            {
                return BadRequest();
            }

            // 已取消的 order detail 不可再 report
            if (od.OrderDetailStatusId == "C") // Cancel
            {
                return BadRequest();
            }

            Report report = new Report
            {
                ReportId = Guid.NewGuid(),
                OrderDetailId = model.OrderDetailId,
                Description = model.Description,
                ReportTime = DateTime.Now,
            };

            _dbContext.Reports.Add(report);

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

        /*
         * /ReportApi/CloseReport
         * 
         */
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CloseReport(CloseReportViewModel model)
        {
            Report report = await _dbContext.Reports
                .FindAsync(model.ReportId);

            if(report == null)
            {
                return NotFound("找不到此問題反映");
            }

            report.CloseTime = DateTime.Now;
            report.Note = model.Note;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict("資料庫更新失敗");
            }

            return Ok();
        }
    }
}
