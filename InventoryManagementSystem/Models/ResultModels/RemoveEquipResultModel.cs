using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class RemoveEquipResultModel
    {
        public int IdToBeRemoved { get; set; }
        public bool Removed { get; set; }
    }
}
