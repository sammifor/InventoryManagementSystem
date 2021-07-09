using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Test()
        {
            return View();
        }

        public IActionResult Test2() 
        {
            return View();
        }
    }
}
