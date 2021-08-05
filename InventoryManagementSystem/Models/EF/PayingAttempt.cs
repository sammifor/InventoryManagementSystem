using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class PayingAttempt
    {
        public string PaymentDetailSn { get; set; }
        public Guid PaymentId { get; set; }

        public virtual Payment Payment { get; set; }
    }
}
