using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class CanceledOrder
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public string Description { get; set; }
        public DateTime? CancelTime { get; set; }

        public virtual Order Order { get; set; }
        public virtual User User { get; set; }
    }
}
