using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class PutUserViewModel : PostUserViewModel
    {
        public string OldPassword { get; set; }
    }
}
