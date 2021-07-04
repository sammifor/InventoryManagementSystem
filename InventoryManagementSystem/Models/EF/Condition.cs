using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Condition
    {
        public Condition()
        {
            ItemLogs = new HashSet<ItemLog>();
            Items = new HashSet<Item>();
        }

        public string ConditionId { get; set; }
        public string ConditionName { get; set; }

        public virtual ICollection<ItemLog> ItemLogs { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}
