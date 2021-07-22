using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class ItemLostCheckViewModel
    {
        public Guid OrderDetailId { get; set; }
        public string Description { get; set; }
    }
}
