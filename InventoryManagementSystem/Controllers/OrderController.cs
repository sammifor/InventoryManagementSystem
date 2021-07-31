using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    [Route("[controller]/[action]")]
    public class OrderController : Controller
    {
        [HttpGet]
        // public IActionResult NewOrder([FromQuery(Name = "id")]int equipId)
        // 後端連接都不用接
        [Authorize(Roles = "user")]
        public IActionResult newOrder()
        {
            return View();
        }
        
        [HttpGet("order")]
        [Authorize]
        public IActionResult OrderQry()
        {
            bool isAdmin = User.IsInRole("admin");

            if(isAdmin)
                return View("orderManageAdmin");
            else
                return View("orderQryUser");
        }

        [Authorize(Roles = "user")]
        public IActionResult orderReport()
        {
            return View();
        }

    }
}
