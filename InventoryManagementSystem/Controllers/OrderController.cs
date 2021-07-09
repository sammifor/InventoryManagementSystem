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
        [HttpPost]
        [Route("{equipId}")]
        public IActionResult NewOrder(int equipId)
        {
            Response.Cookies.Append(
                "selected-equip",   // cookie 的 key
                equipId.ToString(), // cookie 的 value
                new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Secure = true,
                    Expires = DateTimeOffset.Now.AddMinutes(15),
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                    MaxAge = TimeSpan.FromMinutes(15),
                    IsEssential = true
                });
            return View();
        }
    }
}
