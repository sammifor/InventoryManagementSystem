using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.Password;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public AdminController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("adminlogin")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("adminlogin")]
        public async Task<IActionResult> Authenticate(string username, string password)
        {
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return View("Login");
            }

            Admin admin = await _dbContext.Admins
                .Where(a => a.Username == username)
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return View("Login");
            }

            PBKDF2 hasher = new PBKDF2(password, admin.Salt);

            if (hasher.HashedPassword.SequenceEqual(admin.HashedPassword))
            {

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, admin.Username));
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);
                return RedirectToAction("EquipQry", "Equips");
            }
            else
            {
                return View("Login");
            }
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/admin")]
        public IActionResult AdminManagement()
        {
            return View();
        }

        [HttpGet("/addadmin")]
        public IActionResult AddAdmin()
        {
            return View();
        }
    }
}
