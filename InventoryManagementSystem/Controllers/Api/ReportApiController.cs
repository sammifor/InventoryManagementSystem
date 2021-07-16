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
    public class ReportApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public ReportApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<IActionResult> GetReportsByOrderDetailId(int id)
        {
            GetReportsResultModel[] reports = await _dbContext.Reports
                .Where(r => r.OrderDetailId == id)
                .Select(r => new GetReportsResultModel
                {
                    ReportId = r.ReportId,
                    OrderDetailId = r.OrderDetailId,
                    Description = r.Description,
                    ReportTime = r.ReportTime,
                    CloseTime = r.CloseTime
                })
                .ToArrayAsync();

            return Ok(reports);
        }

        /*
         * ReportApi/PostReport
         */
        // 使用者對某筆 order detail 的新增 report
        [HttpPost]
        public async Task<IActionResult> PostReport(PostReportViewModel model)
        {
            // TODO 確認發出 request 的用戶是 order detail 的用戶
            OrderDetail od = await _dbContext.OrderDetails
                .FindAsync(model.OrderDetailId);

            // 不可對不存在的 order detail 新增 report
            if (od == null)
            {
                return NotFound();
            }

            // 已取消的 order detail 不可再 report
            if (od.OrderDetailStatusId == "C") // Cancel
            {
                return BadRequest();
            }

            Report report = new Report
            {
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

    }
}
