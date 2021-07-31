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
        [HttpGet("equip")]
        [Authorize]
        public IActionResult EquipQry()
        {
            bool isAdmin = User.IsInRole("admin");

            if(isAdmin)
                return View("equipQryAdmin");
            else
                return View("equipQryUser");
        }


        [Authorize(Roles = "admin")]
        public IActionResult backendManagement()
        {
            return View();        
        
        }

        [Authorize(Roles = "admin")]
        public IActionResult EquipManagement()
        {

            return View();
        
        }
    }
}
