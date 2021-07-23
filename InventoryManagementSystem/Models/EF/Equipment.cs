using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Equipment
    {
        public Equipment()
        {
            Items = new HashSet<Item>();
            Orders = new HashSet<Order>();
        }

        public Guid EquipmentId { get; set; }
        public Guid EquipmentCategoryId { get; set; }
        public string EquipmentSn { get; set; }
        public string EquipmentName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public decimal UnitPrice { get; set; }
        public string Description { get; set; }

        public virtual EquipCategory EquipmentCategory { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
