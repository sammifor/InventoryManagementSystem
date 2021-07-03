using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models
{
    public partial class LineNotification
    {
        public int LineNotificationId { get; set; }
        public int OrderDetailId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Message { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }
    }
}
