using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class RespondOrderViewModel
    {
        public Guid OrderID { get; set; }
        public bool Reply { get; set; } // 核可給 true；拒絕給 false
        public Guid[] ItemIDs { get; set; }
    }
}
