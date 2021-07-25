using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.PaymentProviderModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        [HttpPost("/payment/result")]
        public async Task<IActionResult> PaymentResult(MPGReturn result)
        {
            if(result.MerchantID != _payConfig.MerchantID || result.Version != _payConfig.Version)
            {
                ViewData["TradeResult"] = "交易資料錯誤";
                return View();
            }

            string hashed = result.GetHashSha256($"HashKey={_payConfig.HashKey}&{result.TradeInfo}&HashIV={_payConfig.HashIV}");

            if(hashed != result.TradeSha)
            {
                ViewData["TradeResult"] = "檢查碼驗證錯誤";
                return View();
            }

            string decryptedInfo = result.DecryptAES256(_payConfig.HashKey, _payConfig.HashIV);
            TradeResult info = JsonConvert.DeserializeObject<TradeResult>(decryptedInfo);

            if(info.Status != "SUCCESS")
            {
                // 其他交易失敗訊息
                ViewData["TradeResult"] = info.Status;
                return View();
            }

            int totalPrice = info.Result.Amt;
            string paymentDetailSn = info.Result.MerchantOrderNo;
            string tradeNo = info.Result.TradeNo;
            string ip = info.Result.IP;
            DateTime payTime = DateTime.Parse(info.Result.PayTime);

            bool isFirstPay = paymentDetailSn.StartsWith('0');

            Guid paymentId;
            if(isFirstPay)
            {
                #region 新增 Payment
                paymentId = Guid.NewGuid();
                Payment payment = new Payment
                {
                    PaymentId = paymentId,
                    RentalFee = totalPrice,
                    ExtraFee = 0
                };
                _dbContext.Payments.Add(payment);
                #endregion

                #region 新增 PaymentLog
                PaymentLog pLog = new PaymentLog
                {
                    PaymentLogId = Guid.NewGuid(),
                    PaymentId = paymentId,
                    Fee = totalPrice,
                    FeeCategoryId = "R", // Rental fee
                    Description = string.Empty
                };
                _dbContext.PaymentLogs.Add(pLog);
                #endregion

                #region 新增 PaymentOrder （一對多關聯表）
                int[] orderSNs = await _dbContext.NewPayingAttempts
                    .Where(npa => npa.PaymentDetailSn == paymentDetailSn)
                    .Select(npa => npa.OrderSn)
                    .ToArrayAsync();

                Guid[] orderIDs = await _dbContext.Orders
                    .Where(o => orderSNs.Contains(o.OrderSn))
                    .Select(o => o.OrderId)
                    .ToArrayAsync();

                foreach(Guid orderID in orderIDs)
                {
                    PaymentOrder po = new PaymentOrder
                    {
                        PaymentId = paymentId,
                        OrderId = orderID
                    };
                    _dbContext.PaymentOrders.Add(po);
                }
                #endregion
            }
            else
            {
                paymentId = await _dbContext.PayingAttempts
                    .Where(pa => pa.PaymentDetailSn == paymentDetailSn)
                    .Select(pa => pa.PaymentId)
                    .FirstOrDefaultAsync();
            }

            #region 新增 PaymentDetail
            PaymentDetail pd = new PaymentDetail
            {
                PaymentDetailId = Guid.NewGuid(),
                PaymentDetailSn = paymentDetailSn,
                PaymentId = paymentId,
                AmountPaid = totalPrice,
                PayTime = payTime,
                TradeNo = tradeNo,
                Ip = ip
            };
            _dbContext.PaymentDetails.Add(pd);
            #endregion

            #region 更新資料庫
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                ViewData["TradeResult"] = "資料庫更新錯誤";
                return View();
            }
            #endregion

            ViewData["TradeResult"] = "交易成功";
            return View();
        }

    }
}
