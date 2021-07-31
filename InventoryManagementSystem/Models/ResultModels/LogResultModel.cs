using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class LogResultModel
    {
        public Guid ItemLogId { get; set; }
        public int ItemLogSn { get; set; }
        public int OrderDetailSn { get; set; } //TODO
        public string AdminUsername { get; set; } //TODO
        public string ItemSn { get; set; }
        public string ConditionName { get; set; }
        public string AdminFullName { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
