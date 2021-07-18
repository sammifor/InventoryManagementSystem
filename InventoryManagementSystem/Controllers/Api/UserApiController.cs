using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.Interfaces;
using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

            return Ok("hi");
        }
    }
}
