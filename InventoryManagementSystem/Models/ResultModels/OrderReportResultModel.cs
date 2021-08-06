using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class OrderReportResultModel
    {

        public DateTime? OrderTime { get; set; }
        public Guid OrderId { get; set; }
        public int OrderSn { get; set; }
        public Guid UserId { get; set; }
        public String Username { get; set; }
        public Guid EquipmentId { get; set; }
        public String EquipmentSn { get; set; }
        public String EquipmentName { get; set; }
        public int Quantity { get; set; }
        public DateTime EstimatedPickupTime { get; set; }
        public String OrderStatusId { get; set; }
        public OrderDetailResultModel[] OrderDetails { get; set; }
    }
}
