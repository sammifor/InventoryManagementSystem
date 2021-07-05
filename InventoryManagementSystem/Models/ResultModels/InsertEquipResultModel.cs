using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class InsertEquipResultModel
    {
        public bool Ok { get; set; }
        public InsertEquipResultModel(bool ok)
        {
            Ok = ok;
        }
    }
}
