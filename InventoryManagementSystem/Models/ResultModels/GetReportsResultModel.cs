using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class GetReportsResultModel
    {
        public Guid ReportId { get; set; }
        public Guid OrderDetailId { get; set; }
        public string Description { get; set; }
        public DateTime? ReportTime { get; set; }
        public DateTime? CloseTime { get; set; }
    }
}
