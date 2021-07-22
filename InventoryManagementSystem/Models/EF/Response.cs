using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Response
    {
        public Guid ResponseId { get; set; }
        public Guid OrderId { get; set; }
        public Guid AdminId { get; set; }
        public string Reply { get; set; }
        public DateTime? ResponseTime { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual Order Order { get; set; }
    }
}
