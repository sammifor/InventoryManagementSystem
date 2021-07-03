using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models
{
    public partial class User
    {
        public User()
        {
            CanceledOrders = new HashSet<CanceledOrder>();
            Orders = new HashSet<Order>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool? AllowNotification { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ViolationTimes { get; set; }
        public bool? Banned { get; set; }
        public string LineAccount { get; set; }

        public virtual ICollection<CanceledOrder> CanceledOrders { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
