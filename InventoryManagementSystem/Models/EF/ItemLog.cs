using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class ItemLog
    {
        public int ItemLogId { get; set; }
        public int? OrderDetailId { get; set; }
        public int? AdminId { get; set; }
        public int ItemId { get; set; }
        public string ConditionId { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTime { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual Condition Condition { get; set; }
        public virtual Item Item { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
    }
}
