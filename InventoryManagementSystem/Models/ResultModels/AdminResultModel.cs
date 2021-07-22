using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class AdminResultModel
    {
        public Guid AdminId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
