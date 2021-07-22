using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            ItemLogs = new HashSet<ItemLog>();
            LineNotifications = new HashSet<LineNotification>();
            Reports = new HashSet<Report>();
        }

        public Guid OrderDetailId { get; set; }
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
        public string OrderDetailStatusId { get; set; }

        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
        public virtual OrderDetailStatus OrderDetailStatus { get; set; }
        public virtual ICollection<ItemLog> ItemLogs { get; set; }
        public virtual ICollection<LineNotification> LineNotifications { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
