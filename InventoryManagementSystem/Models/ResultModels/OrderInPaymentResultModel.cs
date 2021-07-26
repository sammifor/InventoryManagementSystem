using System;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class OrderInPaymentResultModel
    {
        // From Order table
        public Guid OrderId { get; set; }
        public int OrderSn { get; set; }
        public int Quantity { get; set; }
        public DateTime? OrderTime { get; set; }

        // From Equipment table
        public string EquipmentSn { get; set; }
        public string EquipmentName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public decimal? Price { get; set; }

        public string StatusName { get; set; }
    }
}