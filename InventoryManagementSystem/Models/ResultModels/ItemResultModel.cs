using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class ItemBaseResultModel
    {
        public Guid ItemId { get; set; }
        public string ItemSn { get; set; }
        public string Description { get; set; }
    }
    public class ItemResultModel : ItemBaseResultModel
    {
        public string EquipmentName { get; set; }
        public string EquipmentSn { get; set; }
        public string Condition { get; set; }
        // From EquipCategory table
        public string CategoryName { get; set; }
    }
}
