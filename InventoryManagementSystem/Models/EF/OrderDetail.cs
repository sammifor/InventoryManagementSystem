using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            ExtraFees = new HashSet<ExtraFee>();
            ItemLogs = new HashSet<ItemLog>();
            Reports = new HashSet<Report>();
        }

        public Guid OrderDetailId { get; set; }
        public int OrderDetailSn { get; set; }
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
        public string OrderDetailStatusId { get; set; }

        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
        public virtual OrderDetailStatus OrderDetailStatus { get; set; }
        public virtual ICollection<ExtraFee> ExtraFees { get; set; }
        public virtual ICollection<ItemLog> ItemLogs { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
