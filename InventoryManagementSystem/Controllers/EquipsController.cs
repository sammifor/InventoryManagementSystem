using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class EquipsController : Controller
    {
        [Authorize(Roles = "user")]
        public IActionResult equipQryUser()
        {
            return View();
        }
        
        [Authorize(Roles = "admin")]
        public IActionResult equipQryAdmin()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public IActionResult equipAddAdmin() {

            return View();
        
        }

        [Authorize(Roles = "admin")]
        public IActionResult cateAddAdmin()
        {

            return View();

        }

        [Authorize(Roles = "admin")]
        public IActionResult itemAddAdmin()
        {

            return View();

        }

        [Authorize(Roles = "admin")]
        public IActionResult backendManagement()
        {
            return View();        
        
        }

        public IActionResult EquipManagement()
        {

            return View();
        
        }
    }
}
