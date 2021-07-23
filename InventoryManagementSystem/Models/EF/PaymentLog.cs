using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class PaymentLog
    {
        public Guid PaymentLogId { get; set; }
        public string FeeCategoryId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Fee { get; set; }
        public string Description { get; set; }

        public virtual FeeCategory FeeCategory { get; set; }
        public virtual Payment Payment { get; set; }
    }
}
