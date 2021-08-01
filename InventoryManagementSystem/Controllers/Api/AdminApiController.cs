using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.Password;
using InventoryManagementSystem.Models.ResultModels;
using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("api")]
    [ApiController]
    public class AdminApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public AdminApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* GET
         * 
         * api/admin/{AdminID} 找特定 admin
         * api/admin 找所有 admin
         *
         */
        // 查詢所有或特定 admin
        [HttpGet("admin/{id?}")]
        [Produces("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAdmin(Guid? id)
        {
            IQueryable<Admin> qryAdmins = null;
            if(id != null)
                qryAdmins = _dbContext.Admins
                    .Where(a => a.AdminId == id);
            else
                qryAdmins = _dbContext.Admins
                    .Select(a => a);

            AdminResultModel[] admins = await qryAdmins
                .Select(a => new AdminResultModel
                {
                    AdminId = a.AdminId,
                    RoleId = a.RoleId,
                    RoleName = a.Role.RoleName,
                    Username = a.Username,
                    FullName = a.FullName,
                    CreateTime = a.CreateTime
                })
                .ToArrayAsync();

            return Ok(admins);
        }

        /* POST
         * 
         * api/admin
         *
         */
        // 新增 admin
        [HttpPost("admin")]
        [Consumes("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostAdmin(PostAdminViewModel model)
        {
            if(string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Password) ||
                string.IsNullOrWhiteSpace(model.FullName))
            {
                return BadRequest();
            }

            PBKDF2 hasher = new PBKDF2(model.Password, null);

            Admin admin = new Admin
            {
                RoleId = model.RoleId,
                Username = model.Username,
                HashedPassword = hasher.HashedPassword,
                Salt = hasher.Salt,
                FullName = model.FullName,
                CreateTime = DateTime.Now
            };

            _dbContext.Admins.Add(admin);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            return Ok();
        }

        /* PUT
         * 
         * api/admin/{AdminID}
         * 
         */
        // 修改 admin
        [HttpPut("admin/{id}")]
        [Consumes("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutAdmin(Guid id, PutAdminViewModel model)
        {
            if(id != model.AdminId)
            {
                return BadRequest();
            }

            Admin admin = await _dbContext.Admins.FindAsync(id);

            if(admin == null)
            {
                return NotFound();
            }

            admin.RoleId = model.RoleId;
            admin.Username = model.Username;
            admin.FullName = model.FullName;

            string adminIdString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
            Guid adminId = Guid.Parse(adminIdString);

            // 只能改自己的密碼
            if(adminId == id)
            {
                PBKDF2 hasher = new PBKDF2(model.Password, null);

                admin.HashedPassword = hasher.HashedPassword;
                admin.Salt = hasher.Salt;
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            return Ok();
        }

    }
}
