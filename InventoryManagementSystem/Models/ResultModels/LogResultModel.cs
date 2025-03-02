﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class LogResultModel
    {
        public Guid ItemLogId { get; set; }
        public int ItemLogSn { get; set; }
        public int? OrderSn { get; set; }
        public int? OrderDetailSn { get; set; } 
        public string AdminUsername { get; set; } 
        public string ItemSn { get; set; }
        public string ConditionName { get; set; }
        public string AdminFullName { get; set; }
        public string Description { get; set; }
        public string CreateTime { get; set; }
    }
}
