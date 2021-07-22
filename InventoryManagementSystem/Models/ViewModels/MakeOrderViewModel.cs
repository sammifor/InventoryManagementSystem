using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class MakeOrderViewModel
    {
        public Guid EquipmentId { get; set; }
        public int Quantity { get; set; }
        public DateTime EstimatedPickupTime { get; set; }
        public int Day { get; set; }
    }
}
