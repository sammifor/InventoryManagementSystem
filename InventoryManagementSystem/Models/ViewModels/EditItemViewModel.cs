using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class EditItemViewModel
    {
        public Guid ItemId { get; set; }
        public string ItemSn { get; set; }
        public string Description { get; set; }
    }
}
