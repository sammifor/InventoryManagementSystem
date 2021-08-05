using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class User
    {
        public User()
        {
            CanceledOrders = new HashSet<CanceledOrder>();
            Notifications = new HashSet<Notification>();
            Orders = new HashSet<Order>();
        }

        public Guid UserId { get; set; }
        public int UserSn { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public byte[] HashedPassword { get; set; }
        public byte[] Salt { get; set; }
        public string FullName { get; set; }
        public bool? AllowNotification { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ViolationTimes { get; set; }
        public bool? Banned { get; set; }
        public string LineId { get; set; }
        public bool? Deleted { get; set; }

        public virtual ResetPasswordToken ResetPasswordToken { get; set; }
        public virtual ICollection<CanceledOrder> CanceledOrders { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
