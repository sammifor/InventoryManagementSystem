using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.PaymentProviderModels
{
    public class PaymentProviderConfig
    {
        public string HashKey { get; set; }
        public string HashIV { get; set; }
        public string MerchantID { get; set; }
        public string Version { get; set; }
        public string RespondType { get; set; }
        public int LoginType { get; set; }
        public string ClientBackURL { get; set; }
        public string ReturnURL { get; set; }
        public string NotifyURL { get; set; }
    }
}
