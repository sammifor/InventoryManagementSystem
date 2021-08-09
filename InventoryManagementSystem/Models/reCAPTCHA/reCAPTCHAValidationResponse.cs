using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.reCAPTCHA
{
    public class reCAPTCHAValidationResponse
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
        [JsonProperty("error-codes")]
        public string[] errorCodes { get; set; }
    }
}
