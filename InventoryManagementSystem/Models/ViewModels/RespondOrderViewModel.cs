using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class RespondOrderViewModel
    {
        public int OrderID { get; set; }
        public int AdminID { get; set; }
        public string Reply { get; set; } //bool
        public int[] ItemIDs { get; set; }
    }
}
