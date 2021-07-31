using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class PostExtraFeeViewModel
    {
        public Guid OrderDetailId { get; set; }
        public decimal Fee { get; set; }
        public string Description { get; set; }
    }
}
