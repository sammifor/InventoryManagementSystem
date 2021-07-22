using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class ReturnItemsCheckViewModel
    {
        public Guid OrderDetailID { get; set; }
        public bool FunctionsNormally { get; set; } // 是否運作正常？
        public string Description { get; set; } // 詳細描述
    }
}
