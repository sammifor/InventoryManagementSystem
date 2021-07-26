using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.PaymentProviderModels;
using InventoryManagementSystem.Models.ViewModels;
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
            PaymentViewModel model = new PaymentViewModel();

            if(result.MerchantID != _payConfig.MerchantID || result.Version != _payConfig.Version)
            {
                model.Message = "交易資料錯誤";
                return View(model);
            }

            string hashed = result.GetHashSha256($"HashKey={_payConfig.HashKey}&{result.TradeInfo}&HashIV={_payConfig.HashIV}");

            if(hashed != result.TradeSha)
            {
                model.Message = "檢查碼驗證錯誤";
                return View(model);
            }

            string decryptedInfo = result.DecryptAES256(_payConfig.HashKey, _payConfig.HashIV);
            TradeResult info = JsonConvert.DeserializeObject<TradeResult>(decryptedInfo);

            if(info.Status != "SUCCESS")
            {
                // 其他交易失敗訊息
                model.Message = info.Message;
                return View(model);
            }

            int totalPrice = info.Result.Amt;
            string paymentDetailSn = info.Result.MerchantOrderNo;
            string tradeNo = info.Result.TradeNo;
            string ip = info.Result.IP;
            DateTime payTime = DateTime.Parse(info.Result.PayTime);

            string paymentSn = string.Empty;

            bool isFirstPay = paymentDetailSn.StartsWith('0');

            Guid paymentId;
            if(isFirstPay)
            {
                #region 新增 Payment
                paymentId = Guid.NewGuid();

                // paymentSn is created using the first associated paymentDetailSn
                int left = int.Parse(paymentDetailSn.Substring(0, 9));
                int right = int.Parse(paymentDetailSn.Substring(9, 9));
                paymentSn = left.ToString("X") + right.ToString("X");

                Payment payment = new Payment
                {
                    PaymentId = paymentId,
                    PaymentSn = paymentSn,
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
                paymentSn = await _dbContext.Payments
                    .Where(p => p.PaymentId == paymentId)
                    .Select(p => p.PaymentSn)
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
                model.Message = "資料庫更新錯誤";
                return View(model);
            }
            #endregion

            model.Message = "交易成功";
            model.PaymentDetailSn = paymentDetailSn;
            model.PaymentSn = paymentSn;
            model.Success = true;
            model.PayTime = payTime;
            model.Ip = ip;
            return View(model);
        }
    }
}
