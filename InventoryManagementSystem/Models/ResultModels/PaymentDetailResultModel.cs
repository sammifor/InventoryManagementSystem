using System;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class PaymentDetailResultModel
    {
        public int PaymentDetailId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime? PayTime { get; set; }
    }
}