using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }

        
    }
}
