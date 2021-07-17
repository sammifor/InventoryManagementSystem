using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.Interfaces;
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
    public class AdminController : Controller, IHashPassword
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
            Admin admin = await _dbContext.Admins
                .Where(a => a.Username == username)
                .FirstOrDefaultAsync();

            if (admin == null)
            {
                return View("Login");
            }

            var hasher = this as IHashPassword;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedPassword = hasher.HashPasswordWithSalt(passwordBytes, admin.Salt);

            if (hashedPassword.SequenceEqual(admin.HashedPassword))
            {

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString(), ClaimValueTypes.Integer32));
                claims.Add(new Claim(ClaimTypes.Name, admin.Username));
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);
                return RedirectToAction("equipQryAdmin", "Equips");
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

        public IActionResult adminManage()
        {
            return View();
        }

        public IActionResult addAdmin()
        {
            return View();
        }
    }
}
