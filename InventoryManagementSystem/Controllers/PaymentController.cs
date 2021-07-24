using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.PaymentProviderModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentProviderConfig _payConfig;
        private readonly InventoryManagementSystemContext _dbContext;

        public PaymentController(IOptions<PaymentProviderConfig> payConfig, InventoryManagementSystemContext dbContext)
        {
            _payConfig = payConfig.Value;
            _dbContext = dbContext;
        }

        [HttpPost("paying")]
        [Consumes("application/json")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> Paying([FromBody] Guid[] ids)
        {
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

            #region Generating OrderDetailSN for MerchantOrderNo
            DateTimeOffset time = DateTimeOffset.Now;
            string paymentDetailSn = $"{userSn:D5}{time.ToString("yyMMddHHmmssf")}";
            #endregion

            #region 訂單合法且屬於本人

            Guid[] distinctOIDs = ids.Distinct().ToArray();
            if(distinctOIDs.Length != ids.Length)
            {
                return BadRequest("訂單編號有重覆");
            }

            // distinctOIDs 只拿來檢查 ids 是否有重覆
            // 只要能執行到這邊，保證兩個 array 的 elements 都一致
            // 為了不產生混淆，以下一律採用 ids
            Order[] orders = await _dbContext.Orders
                .Where(o => ids.Contains(o.OrderId))
                .Where(o => o.OrderStatusId == "A")
                .Where(o => o.PaymentOrder == null)
                .Where(o => o.EstimatedPickupTime > DateTime.Now)
                .ToArrayAsync();

            // 訂單不合法
            if(orders.Length != ids.Length)
                return BadRequest("有訂單不可執行付款或不存在");

            bool belongToTheUser = orders.All(o => o.UserId == userId);

            // 訂單不屬於本人
            if(!belongToTheUser)
                return BadRequest("有訂單不屬於本人");
            #endregion

            #region 把 OrderSN 資料存在 PayingAttempt Table
            int[] orderSNs = orders.Select(o => o.OrderSn).ToArray();

            foreach(int sn in orderSNs)
            {
                PayingAttempt pa = new PayingAttempt
                {
                    PaymentDetailSn = paymentDetailSn,
                    OrderSn = sn
                };
                _dbContext.PayingAttempts.Add(pa);
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict("資料庫同步錯誤");
            }
            #endregion

            var pricesQry = await (from eq in _dbContext.Equipment
                                   join o in _dbContext.Orders on eq.EquipmentId equals o.EquipmentId
                                   where ids.Contains(o.OrderId)
                                   select eq.UnitPrice * o.Day * o.Quantity)
                             .ToArrayAsync();

            decimal totalPrice = pricesQry
                .Aggregate((total, next) => total + next);





            TradeInfo info = new TradeInfo
            {
                MerchantOrderNo = paymentDetailSn,
                TimeStamp = time.ToUnixTimeSeconds().ToString(),
                Amt = ((int)totalPrice),
                ItemDesc = "租賃費",
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

            ViewData["Price"] = info.Amt;
            return View(mpg);
        }

        [HttpPost("simulator")]
        [Authorize(Roles = "user")]
        public IActionResult LanXinSimulator(string merchantID, string tradeInfo, string tradeSha, string version)
        {
            return Content($"{merchantID}\n{tradeInfo}\n{tradeSha}\n{version}");
        }
    }
}
