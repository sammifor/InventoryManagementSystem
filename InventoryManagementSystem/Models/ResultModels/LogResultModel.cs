using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class LogResultModel
    {
        public int ItemLogId { get; set; }
        public int? OrderDetailId { get; set; }
        public int? AdminId { get; set; }
        public int ItemId { get; set; }
        public string ConditionId { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
