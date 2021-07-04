using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class PaymentCategory
    {
        public PaymentCategory()
        {
            Payments = new HashSet<Payment>();
        }

        public string PaymentCategoryId { get; set; }
        public string PaymentCategoryName { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
