using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class QuestionnaireToken
    {
        public int TokenId { get; set; }
        public byte[] HashedToken { get; set; }
        public Guid OrderId { get; set; }
        public DateTime ExpireTime { get; set; }

        public virtual Order Order { get; set; }
    }
}
