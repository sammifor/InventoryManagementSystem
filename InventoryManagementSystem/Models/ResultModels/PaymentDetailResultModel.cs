using System;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class PaymentDetailResultModel
    {
        public Guid PaymentDetailId { get; set; }
        public string PaymentDetailSn { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime? PayTime { get; set; }
    }
}