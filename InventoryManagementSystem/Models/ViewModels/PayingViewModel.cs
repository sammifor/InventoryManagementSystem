using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class PayingViewModel
    {
        public bool IsRentalFee { get; set; }
        public Guid[] OrderIDs { get; set; }
        public Guid? PaymentID { get; set; }
    }
}
