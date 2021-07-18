using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class PostUserViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool? AllowNotification { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string LineAccount { get; set; }
    }
}
