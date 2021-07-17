using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class OrderDetailResultModel
    {
        public int OrderDetailId { get; set; }
        public int ItemId { get; set; }
        public string ItemSn { get; set; }
        public string OrderDetailStatusId { get; set; }
        public string OrderDetailStatus { get; set; }
    }
}
