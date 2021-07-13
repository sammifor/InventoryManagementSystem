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
        public IActionResult newOrder()
        {
            return View();
        }

        public IActionResult orderQryUser()
        {
            return View();
        }

    }
}
