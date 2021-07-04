using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class OrderDetailStatus
    {
        public OrderDetailStatus()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public string OrderDetailStatusId { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
