using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class MakeOrderMessageResultModel
    {
        // 下單成功為 true；下單失敗為 false
        public bool Ok { get; set; }

        public MakeOrderMessageResultModel(bool ok)
        {
            Ok = ok;
        }
    }
}
