using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class ExtraFeeResultModel
    {
        public string ItemSn { get; set; }
        public int OrderDetailSn { get; set; }
        public decimal Fee { get; set; }
        public string Description { get; set; }
    }
}
