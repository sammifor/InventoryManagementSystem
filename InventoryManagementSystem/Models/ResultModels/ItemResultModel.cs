using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class ItemResultModel
    {
        public int ItemId { get; set; }
        //public int EquipmentId { get; set; }
        public string ItemSn { get; set; }
        //public string ConditionId { get; set; }
        public string Condition { get; set; }
        public string Description { get; set; }
    }
}
