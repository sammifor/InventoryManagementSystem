using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Message { get; set; }

        public virtual User User { get; set; }
    }
}
