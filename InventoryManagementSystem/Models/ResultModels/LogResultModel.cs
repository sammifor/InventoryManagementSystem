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
        public Guid? OrderDetailId { get; set; }
        public int AdminSn { get; set; }
        public Guid? AdminId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemSn { get; set; }
        public string ConditionName { get; set; }
        public string AdminFullName { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
