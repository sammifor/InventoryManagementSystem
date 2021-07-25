using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class NewPayingAttempt
    {
        public string PaymentDetailSn { get; set; }
        public int OrderSn { get; set; }
    }
}
