using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models.EF
{
    public partial class Questionnaire
    {
        public Guid QuestionnaireId { get; set; }
        public Guid OrderId { get; set; }
        public byte SatisfactionScore { get; set; }
        public string Feedback { get; set; }

        public virtual Order Order { get; set; }
    }
}
