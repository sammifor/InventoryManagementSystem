using InventoryManagementSystem.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.Questionnaire
{
    public class TokenValidatorModel
    {
        public bool IsValid { get; set; }
        public QuestionnaireToken QuestionnaireToken { get; set; }
    }
}
