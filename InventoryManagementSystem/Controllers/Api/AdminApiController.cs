using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        // 查詢所有 admin
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAdmin(int id)
        {
            return Ok(id);
        }

        // 新增 admin
        [HttpPost("admin")]
        [Consumes("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostAdmin(PostAdminViewModel model)
        {
            return Ok(model.Username + " post");
        }

        // 修改 admin
        //[HttpPut("admin")]
        //public

    }
}
