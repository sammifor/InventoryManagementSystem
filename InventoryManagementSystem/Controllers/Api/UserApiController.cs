using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.Interfaces;
using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    [Route("api/user")]
    [ApiController]
    public class UserApiController : ControllerBase, IHashPassword
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public UserApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* GET
         * api/user/validate?validatedField={FieldName}&value={Value}
         * 
         * FieldName accepts 3 values: 'username', 'email', 'phoneNumber'.
         * 
         */
        // 驗證 username、email、phoneNumber 是否可被註冊
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateUser(string validatedField, string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return Ok(false);
            }

            bool userExists;
            switch(validatedField)
            {
                case "username":
                    userExists = await _dbContext.Users.AnyAsync(u => u.Username == value);
                    break;
                case "email":
                    userExists = await _dbContext.Users.AnyAsync(u => u.Email == value);
                    break;
                case "phoneNumber":
                    userExists = await _dbContext.Users.AnyAsync(u => u.PhoneNumber == value);
                    break;
                default:
                    return Ok(false);
            }

            if(userExists)
                return Ok(false);

            return Ok(true);
        }
        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PostUser(PostUserViewModel model)
        {
            string[] notNullFields =
            {
                model.Username,
                model.Email,
                model.Password,
                model.FullName,
                model.PhoneNumber
            };

            bool nullOrWhiteSpaceExist = notNullFields
                .Any(f => string.IsNullOrWhiteSpace(f));

            if(nullOrWhiteSpaceExist)
            {
                return BadRequest();
            }

            IHashPassword hasher = this as IHashPassword;
            Random r = new Random();
            byte[] saltBytes = new byte[32];
            byte[] passwordBytes = Encoding.UTF8.GetBytes(model.Password);
            r.NextBytes(saltBytes);
            byte[] hashedPassword = hasher.HashPasswordWithSalt(passwordBytes, saltBytes);

            User user = new User
            {
                Username = model.Username,
                Email = model.Email,
                HashedPassword = hashedPassword,
                Salt = saltBytes,
                FullName = model.FullName,
                AllowNotification = model.AllowNotification,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                CreateTime = DateTime.Now,
                LineAccount = model.LineAccount,
            };

            _dbContext.Users.Add(user);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            // 註冊成功後直接發 cookie，視同登入。
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString(), ClaimValueTypes.Integer32));
            claims.Add(new Claim(ClaimTypes.Name, user.Username));
            claims.Add(new Claim(ClaimTypes.Role, "user"));
            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);
            return RedirectToAction("equipQryUser", "Equips");

        }
    }
}
