﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class EquipResultModel
    {
        public Guid EquipmentId { get; set; }
        public Guid EquipmentCategoryId { get; set; }
        public string EquipmentSn { get; set; }
        public string EquipmentName { get; set; }
        public int QuantityUsable { get; set; }
        public int QuantityInStock { get; set; }
        public int QuantityReserved { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Description { get; set; }

        //From EquipCateogry model
        public string CategoryName { get; set; }

    }
}
