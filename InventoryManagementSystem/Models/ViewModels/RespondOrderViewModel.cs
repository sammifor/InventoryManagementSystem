using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class RespondOrderViewModel
    {
        public int OrderID { get; set; }
        public bool Reply { get; set; } // 核可給 true；拒絕給 false
        public int[] ItemIDs { get; set; }
    }
}
