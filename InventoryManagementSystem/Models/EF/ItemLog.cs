using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class ItemLog
    {
        public Guid ItemLogId { get; set; }
        public int ItemLogSn { get; set; }
        public Guid? OrderId { get; set; }
        public Guid? OrderDetailId { get; set; }
        public Guid? AdminId { get; set; }
        public Guid ItemId { get; set; }
        public string ConditionId { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTime { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual Condition Condition { get; set; }
        public virtual Item Item { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
        public virtual Order Order { get; set; }
    }
}
