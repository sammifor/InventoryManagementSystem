using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class InsertItemViewModel
    {
        public Guid EquipmentId { get; set; }
        public string ItemSn { get; set; }
        public string Description { get; set; }
    }
}
