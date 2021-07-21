using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Questionnaires = new HashSet<Questionnaire>();
            Responses = new HashSet<Response>();
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int EquipmentId { get; set; }
        public int Quantity { get; set; }
        public DateTime EstimatedPickupTime { get; set; }
        public int Day { get; set; }
        public string OrderStatusId { get; set; }
        public DateTime? OrderTime { get; set; }

        public virtual Equipment Equipment { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        public virtual User User { get; set; }
        public virtual CanceledOrder CanceledOrder { get; set; }
        public virtual PaymentOrder PaymentOrder { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Questionnaire> Questionnaires { get; set; }
        public virtual ICollection<Response> Responses { get; set; }
    }
}
