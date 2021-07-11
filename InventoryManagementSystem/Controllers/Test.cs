using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class Test : Controller
    {
        public IActionResult test()
        {
            return View();
        }

        public IActionResult test2()
        {
            return View();
        
        }
    }
}
