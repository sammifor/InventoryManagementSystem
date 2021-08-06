using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Admin
    {
        public Admin()
        {
            ItemLogs = new HashSet<ItemLog>();
            Reports = new HashSet<Report>();
            Responses = new HashSet<Response>();
        }

        public Guid AdminId { get; set; }
        public int AdminSn { get; set; }
        public Guid RoleId { get; set; }
        public string Username { get; set; }
        public byte[] HashedPassword { get; set; }
        public byte[] Salt { get; set; }
        public string FullName { get; set; }
        public DateTime? CreateTime { get; set; }
        public bool Deleted { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<ItemLog> ItemLogs { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
    }
}
