using InventoryManagementSystem.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class PaymentResultModel
    {
        // From Payment table
        public Guid PaymentId { get; set; }
        public decimal RentalFee { get; set; }
        public decimal? ExtraFee { get; set; }

        // From Order table
        public OrderInPaymentResultModel[] Orders { get; set; }

        // from PaymentDetail table
        public PaymentDetailResultModel[] PaymentDetails { get; set; }
    }
}
