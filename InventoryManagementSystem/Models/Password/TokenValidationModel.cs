using InventoryManagementSystem.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.Password
{
    public class TokenValidationModel
    {
        public ResetPasswordToken ResetPasswordToken { get; set; }
        public bool IsValid { get; set; }
    }
}
