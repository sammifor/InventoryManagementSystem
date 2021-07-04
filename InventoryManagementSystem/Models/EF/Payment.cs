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

        public int PaymentId { get; set; }
        public string PaymentCategoryId { get; set; }
        public decimal Fee { get; set; }

        public virtual PaymentCategory PaymentCategory { get; set; }
        public virtual ICollection<PaymentDetail> PaymentDetails { get; set; }
        public virtual ICollection<PaymentOrder> PaymentOrders { get; set; }
    }
}
