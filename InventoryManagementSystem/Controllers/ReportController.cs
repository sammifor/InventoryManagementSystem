﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult ReportGenerate()
        {
            return View();
        }
    }
}
