using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ResultModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("api")]
    [ApiController]
    public class RoleApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public RoleApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* GET
         * 
         * api/role
         * 
         */
        // 取得所有 role
        [HttpGet("role")]
        [Produces("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetRoles()
        {
            RoleResultModel[] roles = await _dbContext.Roles
                .Select(r => new RoleResultModel
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName
                })
                .ToArrayAsync();

            return Ok(roles);
        }
    }
}
