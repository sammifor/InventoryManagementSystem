using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ChartApiController : Controller
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public ChartApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EquipmentCategoryPiechart()
        {
            var results = _dbContext.Orders
                .Where(c => c.PaymentOrder.Payment.PaymentSn != null)
                .Where(c => !c.Equipment.Deleted)
                .GroupBy(c => c.Equipment.EquipmentCategory.CategoryName)
                .Select(c => new {
                    ChartEquipCate = c.Key,
                    ChartEquipCateAmt = c.Sum(a=>a.Quantity)
                    

                }).ToList();

            return Ok(results);
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EquipmentCategoryTop5()
        {
            var results = _dbContext.Orders
                .Where(c => c.PaymentOrder.Payment.PaymentSn != null)
                .Where(c => !c.Equipment.Deleted)
                .GroupBy(c => c.Equipment.EquipmentCategory.CategoryName)
                .Take(5)
                .Select(c => new {
                    ChartEquipCate = c.Key,
                    ChartEquipCateAmt = c.Sum(a => a.Quantity)
                })
                .OrderByDescending(c=>c.ChartEquipCateAmt)
                .ToList();

            return Ok(results);
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EquipmentCategoryOrders()
        {
            var results = _dbContext.EquipCategories
                .SelectMany(c => c.Equipment)
                .SelectMany(e => e.Orders)
                .Where(o => o.OrderStatusId == "E")

                .Select(o => new
                {
                    ChartEquipCate = o.Equipment.EquipmentCategory.CategoryName,
                    ChartEquipCateAmt = o.Quantity * o.Day * o.Equipment.UnitPrice
                })
                .ToList()
                .GroupBy(o => o.ChartEquipCate)
                .Select(g => new 
                {
                    ChartEquipCate = g.Key,
                    ChartEquipCateAmt = g.Sum(o=>o.ChartEquipCateAmt)
                })
                .OrderByDescending(g=>g.ChartEquipCateAmt);
                
            return Ok(results);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EquipmentCategoryOrdersTop5()
        {
            var results = _dbContext.EquipCategories
                .SelectMany(c => c.Equipment)
                .SelectMany(e => e.Orders)
                .Where(o => o.OrderStatusId == "E")

                .Select(o => new
                {
                    ChartEquipCate = o.Equipment.EquipmentCategory.CategoryName,
                    ChartEquipCateAmt = o.Quantity * o.Day * o.Equipment.UnitPrice
                })
                .ToList()
                .GroupBy(o => o.ChartEquipCate)
                .Select(g => new
                {
                    ChartEquipCate = g.Key,
                    ChartEquipCateAmt = g.Sum(o => o.ChartEquipCateAmt)
                })
                .OrderByDescending(g => g.ChartEquipCateAmt)
                .Take(5);

            return Ok(results);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EquipmentCategoryReport()
        {
            var results = _dbContext.Reports
                
                .Select(r => new
                {
                    ChartEquipCate = r.OrderDetail.Item.Equipment.EquipmentCategory.CategoryName,
                    ChartEquipCateReport = r.ReportSn
                })
                .GroupBy(r => r.ChartEquipCate)
                .Select(g => new 
                {
                    ChartEquipCate = g.Key,
                    ChartEquipCateAmt = g.Count()
                })
                .ToList();
                

            return Ok(results);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EquipmentCategoryReportTop5()
        {
            var results = _dbContext.EquipCategories
                .Where(ec=>!ec.Deleted)
                .Select(ec => new
                {
                    ChartEquipCate = ec.CategoryName,
                    ChartEquipCateAmt = ec.Equipment
                    .SelectMany(e => e.Orders)
                    .SelectMany(o => o.OrderDetails)
                    .SelectMany(od => od.Reports)
                    .Count()
                })
                .OrderByDescending(g => g.ChartEquipCateAmt)
                .ToList().Take(5);

            return Ok(results);
        }

        //設備種類狀態
        [HttpGet("{status}")]
        [Authorize(Roles = "admin")]
        public IActionResult EquipmentCategoryStatus(string status)
        {
            string[] validInputs = { "I", "O", "P"};
            if (!validInputs.Contains(status))
                return BadRequest();

            var results = _dbContext.EquipCategories
                .Where(ec => !ec.Deleted)
                .Select(ec => new
                {
                    ChartEquipCate = ec.CategoryName,
                    ChartEquipCateAmt = ec.Equipment
                    .SelectMany(e => e.Items)
                    .Count(i => i.ConditionId == status)
                })
                .ToList();


            return Ok(results);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EquipmentCategoryBroken()
        {
            var results = _dbContext.Items
                .Select(i => new
                {
                    ChartEquipCate = i.Equipment.EquipmentCategory.CategoryName,
                    ChartEquipStatus = i.ConditionId
                })
                .GroupBy(i => i.ChartEquipCate)
                .Select(g => new
                {
                    ChartEquipCate = g.Key,
                    ChartEquipCateAmt = g.Count(i => i.ChartEquipStatus == "F")
                })
                .Where(b => b.ChartEquipCateAmt > 0)
                .OrderByDescending(b => b.ChartEquipCateAmt)
                .ToList();

            return Ok(results);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EquipmentCategoryBrokenTop5()
        {
            var results = _dbContext.Items
                .Select(i => new
                {
                    ChartEquipCate = i.Equipment.EquipmentCategory.CategoryName,
                    ChartEquipStatus = i.ConditionId
                })
                .ToList()
                .GroupBy(i => i.ChartEquipCate)
                .Select(g => new
                {
                    ChartEquipCate = g.Key,
                    ChartEquipCateAmt = g.Count(i=>i.ChartEquipStatus=="F")
                })
                .OrderByDescending(g => g.ChartEquipCateAmt).Take(5);

            return Ok(results);
        }
    }
}
