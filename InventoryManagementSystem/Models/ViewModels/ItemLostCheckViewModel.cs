using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class ItemLostCheckViewModel
    {
        public int OrderDetailId { get; set; }
        public string Description { get; set; }
    }
}
