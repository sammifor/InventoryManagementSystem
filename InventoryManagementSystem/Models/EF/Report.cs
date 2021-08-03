using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Report
    {
        public Guid ReportId { get; set; }
        public Guid OrderDetailId { get; set; }
        public string Description { get; set; }
        public DateTime? ReportTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public int ReportSn { get; set; }
        public string Note { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }
    }
}
