using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models
{
    public partial class EquipCategory
    {
        public EquipCategory()
        {
            Equipment = new HashSet<Equipment>();
        }

        public int EquipCategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; }
    }
}
