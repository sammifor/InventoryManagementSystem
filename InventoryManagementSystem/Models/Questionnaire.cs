using System;
using System.Collections.Generic;

#nullable disable

namespace InventoryManagementSystem.Models
{
    public partial class Questionnaire
    {
        public int QuestionnaireId { get; set; }
        public int OrderId { get; set; }
        public byte SatisfactionScore { get; set; }
        public string Feedback { get; set; }

        public virtual Order Order { get; set; }
    }
}
