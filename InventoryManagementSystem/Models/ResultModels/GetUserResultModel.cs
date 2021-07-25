using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ResultModels
{
    public class GetUserResultModel
    {
        public Guid UserId { get; set; }
        public int UserSn { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool? AllowNotification { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? ViolationTimes { get; set; }
        public bool? Banned { get; set; }
        public string LineAccount { get; set; }
    }
}
