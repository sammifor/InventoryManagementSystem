using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Report
    {
        public int ReportId { get; set; }
        public int OrderDetailId { get; set; }
        public string Description { get; set; }
        public DateTime? ReportTime { get; set; }
        public DateTime? CloseTime { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }
    }
}
