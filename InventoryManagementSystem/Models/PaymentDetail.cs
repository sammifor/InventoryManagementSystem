using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models
{
    public partial class PaymentDetail
    {
        public int PaymentDetail1 { get; set; }
        public int PaymentId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime? PayTime { get; set; }

        public virtual Payment Payment { get; set; }
    }
}
