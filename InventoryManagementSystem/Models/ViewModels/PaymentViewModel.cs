using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class PaymentViewModel
    {
        public string PaymentDetailSn { get; set; }
        public string PaymentSn { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int TotalPrice { get; set; }
        public string Ip { get; set; }
        public DateTime PayTime { get; set; }
    }
}
