using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Item
    {
        public Item()
        {
            ItemLogs = new HashSet<ItemLog>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid ItemId { get; set; }
        public Guid EquipmentId { get; set; }
        public string ConditionId { get; set; }
        public string ItemSn { get; set; }
        public string Description { get; set; }

        public virtual Condition Condition { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual ICollection<ItemLog> ItemLogs { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
