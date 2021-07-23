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
            PaymentLogs = new HashSet<PaymentLog>();
            PaymentOrders = new HashSet<PaymentOrder>();
        }

        public Guid PaymentId { get; set; }
        public decimal RentalFee { get; set; }
        public decimal? ExtraFee { get; set; }

        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; }
        public virtual ICollection<PaymentLog> PaymentLogs { get; set; }
        public virtual ICollection<PaymentOrder> PaymentOrders { get; set; }
    }
}
