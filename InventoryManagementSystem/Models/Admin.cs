using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models
{
    public partial class Admin
    {
        public Admin()
        {
            ItemLogs = new HashSet<ItemLog>();
            Responses = new HashSet<Response>();
        }

        public int AdminId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime? CreateTime { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<ItemLog> ItemLogs { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
    }
}
