using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.PaymentProviderModels;
using InventoryManagementSystem.Models.ResultModels;
using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
        private readonly PaymentProviderConfig _payConfig;

        public PaymentApiController(InventoryManagementSystemContext dbContext, IOptions<PaymentProviderConfig> payConfig)
        {
            _dbContext = dbContext;
            _payConfig = payConfig.Value;
        }

        /* method: GET
         * 
         * url: /api/payment
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
                        PaymentSn = po.Payment.PaymentSn,
                        RentalFee = po.Payment.RentalFee,

                        Orders = po.Payment.PaymentOrders
                            .Select(po => new OrderInPaymentResultModel
                            {
                                OrderId = po.OrderId,
                                OrderSn = po.Order.OrderSn,
                                Quantity = po.Order.Quantity,
                                OrderTime = po.Order.OrderTime,
                                EquipmentSn = po.Order.Equipment.EquipmentSn,
                                EquipmentName = po.Order.Equipment.EquipmentName,
                                Brand = po.Order.Equipment.Brand,
                                Model = po.Order.Equipment.Brand,
                                Price = po.Order.Quantity * po.Order.Day * po.Order.Equipment.UnitPrice,
                                StatusName = po.Order.OrderStatus.StatusName,
                                // TODO 測試
                                ExtraFees = po.Order.OrderDetails
                                    .SelectMany(od => od.ExtraFees)
                                    .Select(f => new ExtraFeeResultModel
                                    {
                                        ItemSn = f.OrderDetail.Item.ItemSn,
                                        OrderDetailSn = f.OrderDetail.OrderDetailSn,
                                        Fee = f.Fee,
                                        Description = f.Description
                                    })
                                    .ToArray()
                            })
                            .ToArray(),

                        PaymentDetails = po.Payment.PaymentDetails
                            .Select(pd => new PaymentDetailResultModel
                            {
                                PaymentDetailId = pd.PaymentDetailId,
                                PaymentDetailSn = pd.PaymentDetailSn,
                                AmountPaid = pd.AmountPaid,
                                PayTime = pd.PayTime
                            })
                            .ToArray(),
                    })
                    .ToArrayAsync();

            payments = payments
                .GroupBy(p => p.PaymentId)
                .Select(group => group.FirstOrDefault())
                .ToArray();

            foreach(PaymentResultModel p in payments)
            {
                p.Received = p.PaymentDetails
                    .Select(pd => pd.AmountPaid)
                    .Aggregate((curr, next) => curr + next);

                p.ExtraFee = p.Orders
                    .SelectMany(o => o.ExtraFees)
                    .Select(od => od.Fee)
                    .Aggregate(0m, (curr, next) => curr + next, sum => sum);

                p.OutstandingBalance = p.RentalFee + p.ExtraFee - p.Received;

                p.Completed = p.OutstandingBalance == 0 ? true : false;
            }

            return Ok(payments);
        }

        /* method: POST
         * 
         * url: api/payment/new
         * 
         * intput: 
         * 
         *      付 Orders
         *          {
         *              "isRentalFee": true,
         *              "orderIDs": ["...", "...", "...", ...]
         *          }
         *      
         *      付 Payment（只在有 extra fee 的情況）
         *          {
         *              "isRentalFee": false,
         *              "paymentID": "..."
         *          }
         * 
         * output:
         *      {
         *          "merchantID": "...",
         *          "tradeInfo": "...",
         *          "tradeSha": "...",
         *          "version": "...",
         *          "totalPrice": 0
         *      }
         * 
         */
        // 使用者選取 Orders 準備付款、使用者選取 Payment 準備付款
        [HttpPost]
        [Consumes("application/json")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Paying(PayingViewModel model)
        {
            #region Validate the input
            if(model.IsRentalFee && (model.OrderIDs == null || model.OrderIDs.Length == 0))
                return BadRequest("輸入格式錯誤");
            if(!model.IsRentalFee && model.PaymentID == null)
                return BadRequest("輸入格式錯誤");
            #endregion

            // userId, userSn, email
            #region Get user info
            string userIdString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
            string userSnString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)
                .Value;
            string email = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)
                .Value;

            Guid userId = Guid.Parse(userIdString);
            int userSn = int.Parse(userSnString);
            #endregion

            #region Generating PaymentDetailSN for MerchantOrderNo
            DateTimeOffset time = DateTimeOffset.Now;
            string paymentDetailSn = string.Empty;
            // Initial payment starts with 0, and extra payment starts with 1
            if(model.IsRentalFee)
                paymentDetailSn = $"0{userSn:D4}{time.ToString("yyMMddHHmmssf")}";
            else
                paymentDetailSn = $"1{userSn:D4}{time.ToString("yyMMddHHmmssf")}";
            #endregion

            if(model.IsRentalFee)
            {
                #region 訂單合法且屬於本人

                Guid[] distinctOIDs = model.OrderIDs.Distinct().ToArray();
                if(distinctOIDs.Length != model.OrderIDs.Length)
                {
                    return BadRequest("訂單編號有重覆");
                }

                // distinctOIDs 只拿來檢查 ids 是否有重覆
                // 只要能執行到這邊，保證兩個 array 的 elements 都一致
                // 為了不產生混淆，以下一律採用 ids
                Order[] orders = await _dbContext.Orders
                    .Where(o => model.OrderIDs.Contains(o.OrderId))
                    .Where(o => o.OrderStatusId == "A")
                    .Where(o => o.PaymentOrder == null)
                    .Where(o => o.EstimatedPickupTime > DateTime.Now)
                    .ToArrayAsync();

                // 訂單不合法
                if(orders.Length != model.OrderIDs.Length)
                    return BadRequest("有訂單不可執行付款或不存在");

                bool belongToTheUser = orders.All(o => o.UserId == userId);

                // 訂單不屬於本人
                if(!belongToTheUser)
                    return BadRequest("有訂單不屬於本人");
                #endregion

                #region 把 OrderSN 資料存在 NewPayingAttempt Table
                int[] orderSNs = orders.Select(o => o.OrderSn).ToArray();

                foreach(int sn in orderSNs)
                {
                    NewPayingAttempt pa = new NewPayingAttempt
                    {
                        PaymentDetailSn = paymentDetailSn,
                        OrderSn = sn
                    };
                    _dbContext.NewPayingAttempts.Add(pa);
                }
                #endregion
            }
            else
            {
                #region Payment 合法且屬於本人
                bool isValidPayment = await _dbContext.Payments
                    .Where(p => p.PaymentId == model.PaymentID)
                    .AnyAsync(p => p.PaymentOrders.First().Order.UserId == userId);

                if(!isValidPayment)
                    return BadRequest("付款單不存在或不屬於本人");
                #endregion

                #region 把 PaymentDetailSN 資料存在 PayingAttempt Table
                PayingAttempt payingAttempt = new PayingAttempt
                {
                    PaymentDetailSn = paymentDetailSn,
                    PaymentId = model.PaymentID.GetValueOrDefault()
                };

                _dbContext.PayingAttempts.Add(payingAttempt);
                #endregion
            }

            #region 更新資料庫
            try
            {
            await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict("資料庫同步錯誤");
            }
            #endregion

            decimal totalPrice;
            string itemDesc;
            if(model.IsRentalFee)
            {
                itemDesc = "租賃費";
                var pricesQry = await (from eq in _dbContext.Equipment
                                       join o in _dbContext.Orders on eq.EquipmentId equals o.EquipmentId
                                       where model.OrderIDs.Contains(o.OrderId)
                                       select eq.UnitPrice * o.Day * o.Quantity)
                                 .ToArrayAsync();

                totalPrice = pricesQry
                    .Aggregate((total, next) => total + next);
            }
            else
            {
                itemDesc = "補繳費";
                decimal extraReceived = await _dbContext.PaymentDetails
                    .Where(pd => pd.PaymentId == model.PaymentID.GetValueOrDefault())
                    .Where(pd => EF.Functions.Like(pd.PaymentDetailSn, "1%"))
                    .Select(pd => pd.AmountPaid)
                    .SumAsync();

                decimal extraReceivable = await _dbContext.PaymentOrders
                    .Where(po => po.PaymentId == model.PaymentID.GetValueOrDefault())
                    .SelectMany(po => po.Order.OrderDetails)
                    .SelectMany(od => od.ExtraFees)
                    .Select(f => f.Fee)
                    .SumAsync();

                totalPrice = extraReceivable - extraReceived;

                if(totalPrice <= 0)
                    return BadRequest("不需付款");
            }




            TradeInfo info = new TradeInfo
            {
                MerchantOrderNo = paymentDetailSn,
                TimeStamp = time.ToUnixTimeSeconds().ToString(),
                Amt = ((int)totalPrice),
                ItemDesc = itemDesc,
                Email = email,

                MerchantID = _payConfig.MerchantID,
                Version = _payConfig.Version,
                LoginType = _payConfig.LoginType,
                RespondType = _payConfig.RespondType,
                ClientBackURL = _payConfig.ClientBackURL,
                ReturnURL = _payConfig.ReturnURL,
                NotifyURL = _payConfig.NotifyURL
            };

            MPG mpg = new MPG
            {
                MerchantID = _payConfig.MerchantID,
                Version = _payConfig.Version
            };
            mpg.SetTradeInfo(
                info.ToQueryString(),
                _payConfig.HashKey,
                _payConfig.HashIV);

            return Ok(new { 
                mpg.MerchantID,
                mpg.TradeInfo,
                mpg.TradeSha,
                mpg.Version,
                totalPrice
            });
        }

    }
}
