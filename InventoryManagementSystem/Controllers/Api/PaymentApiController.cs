using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ResultModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers.Api
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentApiController : ControllerBase
    {
        private readonly InventoryManagementSystemContext _dbContext;

        public PaymentApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* method: GET
         * 
         * url: api/payment
         * 
         * input: none
         * 
         * output: A JSON object having the same structure 
         *         as PaymentResultModel class.
         * 
         * Note: A user can only get their payment info; while
         *       an admin can get everyone's.
         */
        // 取得 Payment 資訊
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPayment()
        {
            bool isAdmin = User.HasClaim(ClaimTypes.Role, "admin");

            IQueryable<PaymentOrder> paymentOrders = _dbContext.PaymentOrders;

            if(!isAdmin)
            {
                string userIdString = User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                    .Value;

                Guid userId = Guid.Parse(userIdString);

                paymentOrders = paymentOrders
                    .Where(po => po.Order.UserId == userId);
            }

            PaymentResultModel[] payments = await paymentOrders
                    .Select(po => new PaymentResultModel
                    {
                        PaymentId = po.PaymentId,
                        RentalFee = po.Payment.RentalFee,
                        ExtraFee = po.Payment.ExtraFee,

                        Orders = po.Payment.PaymentOrders
                            .Select(po => new OrderInPaymentResultModel
                            {
                                OrderId = po.OrderId,
                                Quantity = po.Order.Quantity,
                                OrderTime = po.Order.OrderTime,
                                EquipmentSn = po.Order.Equipment.EquipmentSn,
                                EquipmentName = po.Order.Equipment.EquipmentName,
                                Brand = po.Order.Equipment.Brand,
                                Model = po.Order.Equipment.Brand,
                                Price = po.Order.Quantity * po.Order.Day * po.Order.Equipment.UnitPrice
                            })
                            .ToArray(),

                        PaymentDetails = po.Payment.PaymentDetails
                            .Select(pd => new PaymentDetailResultModel
                            {
                                PaymentDetailId = pd.PaymentDetailId,
                                AmountPaid = pd.AmountPaid,
                                PayTime = pd.PayTime
                            })
                            .ToArray()
                    })
                    .ToArrayAsync();

            return Ok(payments);
        }
    }
}
