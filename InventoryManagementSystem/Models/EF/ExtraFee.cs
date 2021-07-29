using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class ExtraFee
    {
        public Guid ExtraFeeId { get; set; }
        public int ExtraFeeSn { get; set; }
        public Guid OrderDetailId { get; set; }
        public decimal Fee { get; set; }
        public string Description { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }
    }
}
