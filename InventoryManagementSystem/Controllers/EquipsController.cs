using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inv.Controllers
{
    public class EquipsController : Controller
    {
        public IActionResult equipQry()
        {
            return View();
        }
    }
}
