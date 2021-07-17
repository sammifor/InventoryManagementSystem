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
        
        [Authorize(Roles = "admin")]
        public IActionResult orderManageAdmin()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        public IActionResult orderQryUser()
        {
            return View();
        }


        [Authorize(Roles = "user")]
        public IActionResult orderReport()
        {
            return View();
        }

    }
}
