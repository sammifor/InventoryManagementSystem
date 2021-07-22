using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class EquipCategory
    {
        public EquipCategory()
        {
            Equipment = new HashSet<Equipment>();
        }

        public Guid EquipCategoryId { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; }
    }
}
