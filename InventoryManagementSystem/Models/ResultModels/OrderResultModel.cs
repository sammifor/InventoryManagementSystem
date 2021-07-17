using InventoryManagementSystem.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class OrderResultModel
    {
        // From Order table
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int EquipmentId { get; set; }
        public int Quantity { get; set; }
        public DateTime EstimatedPickupTime { get; set; }
        public int Day { get; set; }
        public string OrderStatusId { get; set; }
        public DateTime? OrderTime { get; set; }

        // From Equipment table
        public string EquipmentSn { get; set; }
        public string EquipmentName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Description { get; set; }

        // From User table
        public string Username { get; set; }
        
        // From OrderStatus table
        public string StatusName { get; set; }

        public string TabName { get; set; }

        public OrderDetailResultModel[] OrderDetails { get; set; }
    }
}
