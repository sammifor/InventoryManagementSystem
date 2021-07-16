using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class GetOrdersViewModel
    {
        public string Tab { get; set; } // 所有訂單、待核可、待領取、租借中、已結束、已逾期

        // 目前先給前端方便測試用，之後會在後端用 authentication 取 user id。
        // 所以以後 authentication 加進來後，就不用再傳 user id 進來了。
        // 有傳 userid：查該 user 的 order
        // 沒有傳 userid：查所有 user 的 order
        public int? UserId { get; set; } 
    }
}
