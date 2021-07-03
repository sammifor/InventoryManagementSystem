using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models
{
    public partial class Response
    {
        public int ResponseId { get; set; }
        public int OrderId { get; set; }
        public int AdminId { get; set; }
        public string Reply { get; set; }
        public DateTime? ResponseTime { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual Order Order { get; set; }
    }
}
