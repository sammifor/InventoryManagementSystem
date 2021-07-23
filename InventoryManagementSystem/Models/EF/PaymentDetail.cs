using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class PaymentDetail
    {
        public Guid PaymentDetailId { get; set; }
        public string PaymentDetailSn { get; set; }
        public Guid PaymentId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime? PayTime { get; set; }

        public virtual Payment Payment { get; set; }
    }
}
