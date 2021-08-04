using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.Questionnaire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("api/questionnaire")]
    [ApiController]
    public class QuestionnaireApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public QuestionnaireApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("validatetoken")]
        public async Task<IActionResult> ValidateToken([FromForm] string token)
        {
            TokenValidatorModel model = await TokenValidator(token);

            if(!model.IsValid)
                return NotFound();

            return Ok();
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostQuestionnaire(
            [FromForm] string token,
            [FromForm] byte satisfactionScore,
            [FromForm] string feedback)
        {
            TokenValidatorModel model = await TokenValidator(token);

            if(!model.IsValid)
                return NotFound();

            if(satisfactionScore <= 0 || satisfactionScore > 5)
                return BadRequest();

            Questionnaire questionnaire = new Questionnaire
            {
                QuestionnaireId = Guid.NewGuid(),
                OrderId = model.QuestionnaireToken.OrderId,
                Feedback = feedback,
                SatisfactionScore = satisfactionScore
            };

            _dbContext.Questionnaires.Add(questionnaire);
            _dbContext.QuestionnaireTokens.Remove(model.QuestionnaireToken);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            return Ok();
        }

        private async Task<TokenValidatorModel> TokenValidator(string token)
        {
            TokenValidatorModel model = new TokenValidatorModel();

            byte[] tokenBytes = Convert.FromBase64String(token);
            byte[] hashedToken = SHA512.HashData(tokenBytes);

            QuestionnaireToken questionnaireToken = await _dbContext.QuestionnaireTokens
                .Where(qt => qt.HashedToken.SequenceEqual(hashedToken))
                .FirstOrDefaultAsync();

            if(questionnaireToken == null || questionnaireToken.ExpireTime <= DateTime.Now)
            {
                return model;
            }

            model.IsValid = true;
            model.QuestionnaireToken = questionnaireToken;
            return model;
        }
    }
}
