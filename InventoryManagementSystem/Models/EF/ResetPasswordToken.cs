using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class ResetPasswordToken
    {
        public int TokenId { get; set; }
        public Guid UserId { get; set; }
        public byte[] HashedToken { get; set; }
        public byte[] Salt { get; set; }
        public DateTime ExpireTime { get; set; }

        public virtual User User { get; set; }
    }
}
