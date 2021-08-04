using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class QuestionnaireController : Controller
    {
        //滿意度調查問卷
        [HttpGet("/questionnaire")]
        public IActionResult questionnaire()
        {
            return View();
        }
    }
}
