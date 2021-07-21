using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class FeeCategory
    {
        public FeeCategory()
        {
            PaymentLogs = new HashSet<PaymentLog>();
        }

        public string FeeCategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PaymentLog> PaymentLogs { get; set; }
    }
}
