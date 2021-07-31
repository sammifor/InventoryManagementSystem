using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.LINE
{
    public class PushMessage
    {
        public string to { get; set; }
        public LineMessage[] messages { get; set; }
    }

    public class LineMessage
    {
        public string type { get; set; }
        public string text { get; set; }
    }
}
