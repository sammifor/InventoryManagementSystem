using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class OrderDetailResultModel
    {
        public Guid OrderDetailId { get; set; }
        public int OrderDetailSn { get; set; }
        public Guid ItemId { get; set; }
        public string ItemSn { get; set; }
        public int OpenReportCounts { get; set; }
        public string ItemDescription { get; set; }
        public string OrderDetailStatusId { get; set; }
        public string OrderDetailStatus { get; set; }

        // From ItemLog
        public string ItemStatus { get; set; }
    }
}
