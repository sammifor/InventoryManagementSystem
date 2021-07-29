using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Payment
    {
        public Payment()
        {
            PaymentDetails = new HashSet<PaymentDetail>();
            PaymentOrders = new HashSet<PaymentOrder>();
        }

        public Guid PaymentId { get; set; }
        public string PaymentSn { get; set; }
        public decimal RentalFee { get; set; }

        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; }
        public virtual ICollection<PaymentOrder> PaymentOrders { get; set; }
    }
}
