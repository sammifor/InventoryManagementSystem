using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ResultModels;
using InventoryManagementSystem.Models.ViewModels;
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

    [Route("[controller]/[action]")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {

        private readonly InventoryManagementSystemContext _dbContext;

        public OrderApiController(InventoryManagementSystemContext dbContext)
        {
            _dbContext = dbContext;
        }


        /*
         * OrderApi/MakeOrder
         */
        // 下訂單
        [HttpPost]
        [Consumes("application/json")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> MakeOrder(MakeOrderViewModel model)
        {
            if(model.EstimatedPickupTime < DateTime.Today)
            {
                return BadRequest();
            }

            // Get UserID
            string userIDString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
            Guid orderId = Guid.NewGuid();
            Order order = new Order
            {
                UserId = Guid.Parse(userIDString),
                EquipmentId = model.EquipmentId,
                Quantity = model.Quantity,
                EstimatedPickupTime = model.EstimatedPickupTime,
                Day = model.Day,

                // 前端沒權限給的
                OrderStatusId = "P",
                OrderTime = DateTime.Now,
                OrderId = orderId
            };

            _dbContext.Orders.Add(order);

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

        [HttpGet]
        [Produces("application/json")]
        // 所有訂單、待核可、待付款、待領取、租借中、已結束、已逾期
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            IQueryable<Order> tempOrders = null;

            //  Could also use User.IsInRole("user")
            if(User.HasClaim(ClaimTypes.Role, "user"))
            {
                // User 只看得到自己的 Order
                string userIdString = User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                    .Value;
                Guid userId = Guid.Parse(userIdString);

                tempOrders = _dbContext.Orders
                    .Where(o => o.UserId == userId);
            }
            else if(User.HasClaim(ClaimTypes.Role, "admin"))
            {
                // 管理員可以看到所有人的 Order
                tempOrders = _dbContext.Orders.Select(o => o);
            }

            OrderResultModel[] orders = await tempOrders.Select(o => new OrderResultModel
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                EquipmentId = o.EquipmentId,
                Quantity = o.Quantity,
                EstimatedPickupTime = o.EstimatedPickupTime,
                Day = o.Day,
                OrderStatusId = o.OrderStatusId,
                OrderTime = o.OrderTime,

                EquipmentSn = o.Equipment.EquipmentSn,
                EquipmentName = o.Equipment.EquipmentName,
                Brand = o.Equipment.Brand,
                Model = o.Equipment.Model,
                UnitPrice = o.Equipment.UnitPrice,
                Description = o.Equipment.Description,

                Username = o.User.Username,

                StatusName = o.OrderStatus.StatusName,

                OrderDetails = o.OrderDetails.Select(od => new OrderDetailResultModel
                {
                    OrderDetailId = od.OrderDetailId,
                    ItemId = od.ItemId,
                    ItemSn = od.Item.ItemSn,

                    OrderDetailStatusId = od.OrderDetailStatusId,
                    OrderDetailStatus = od.OrderDetailStatus.StatusName
                })
                        .ToArray(),

                PaymentId = o.PaymentOrder.PaymentId
            })
                .ToArrayAsync();

            //"待核可"
            OrderResultModel[] pendingOrders = orders.Where(o =>
                o.OrderStatusId == "P" && // Pending
                o.EstimatedPickupTime > DateTime.Now) // 尚未過期的 order
                .ToArray();

            //"待付款"
            OrderResultModel[] outstandingOrders = orders.Where(o =>
            o.OrderStatusId == "A" && // Order 是核可的
            o.PaymentId == null && // 沒付款
            o.EstimatedPickupTime > DateTime.Now) // 現在還沒過取貨時間
                .ToArray();

            //"待領取"
            OrderResultModel[] readyOrders = orders.Where(o =>
                o.OrderStatusId == "A" && // Order 是核可的
                o.OrderDetails.Any(od => od.OrderDetailStatusId == "P") && // order 底下的 order detail 有待取貨的
                o.EstimatedPickupTime.AddDays(o.Day) > DateTime.Now && // 沒過期的
                o.PaymentId != null) // 已付款
                .ToArray();

            //"租借中"
            OrderResultModel[] ongoinOrders = orders.Where(o =>
                o.OrderStatusId == "A" && // 被核准的 order
                o.OrderDetails.Any(od => od.OrderDetailStatusId == "T") && // 所訂的物品已被取貨
                o.EstimatedPickupTime.AddDays(o.Day) > DateTime.Now) // 尚未逾期
                .ToArray();

            //"已結束"
            string[] restrictions = { "C", "E", "D" }; // Canceled, ended and denied.
            OrderResultModel[] closedOrders = orders.Where(o =>
                restrictions.Contains(o.OrderStatusId) || // 已取消、已結束、已拒絕
                o.OrderStatusId == "P" && o.EstimatedPickupTime < DateTime.Now) // 逾期回應
                .ToArray();

            //"已逾期"
            OrderResultModel[] overdueOrders = orders.Where(o =>
                o.OrderStatusId == "A" &&
                o.OrderDetails.Any(od => od.OrderDetailStatusId == "T") &&
                o.EstimatedPickupTime.AddDays(o.Day) < DateTime.Now)
                .ToArray();

            Array.ForEach(pendingOrders, o => o.TabName = "待核可");
            Array.ForEach(outstandingOrders, o => o.TabName = "待付款");
            Array.ForEach(readyOrders, o => o.TabName = "待領取");
            Array.ForEach(ongoinOrders, o => o.TabName = "租借中");
            Array.ForEach(closedOrders, o => o.TabName = "已結束");
            Array.ForEach(overdueOrders, o => o.TabName = "已逾期");

            orders = pendingOrders
                .Concat(outstandingOrders)
                .Concat(readyOrders)
                .Concat(ongoinOrders)
                .Concat(closedOrders)
                .Concat(overdueOrders)
                .ToArray();


            return Ok(orders);

        }

        /*
         * OrderApi/RespondOrder
         */
        // Admin 核准或拒絕訂單
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RespondOrder(RespondOrderViewModel model)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == model.OrderID &&
                    o.OrderStatusId == "P" && // 待審核的 order
                    o.EstimatedPickupTime > DateTime.Now && // 尚未過期
                    o.PaymentOrder == null); // 尚未付款

            // 找不到訂單
            if(order == null)
            {
                return NotFound();
            }

            // 防止同個 item 被分配多次
            Guid[] itemIDs = model.ItemIDs.Distinct().ToArray();

            Item[] items = await _dbContext.Items
                .Where(i => itemIDs.Contains(i.ItemId))
                .ToArrayAsync();

            // Get AdminID
            string adminIdString = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                .Value;
            Guid adminId = Guid.Parse(adminIdString);

            Response response = new Response
            {
                ResponseId = Guid.NewGuid(),
                OrderId = model.OrderID,
                AdminId = adminId
            };

            if(model.Reply == true)
            {
                // 訂單寫的數量與實際分配的數量不一致
                if(order.Quantity != itemIDs.Length)
                {
                    return BadRequest();
                }

                // 存在有分配的設備非訂單所寫的設備
                bool invalidEquipIdExists = items
                    .Any(i => i.EquipmentId != order.EquipmentId);
                
                if(invalidEquipIdExists)
                {
                    return BadRequest();
                }

                // 庫存不夠，無法滿足訂單
                int inStockNumber = await _dbContext.Items
                    .AsNoTracking()
                    .Where(i => i.EquipmentId == order.EquipmentId)
                    .CountAsync(i => i.ConditionId == "I");
                if(inStockNumber < order.Quantity)
                {
                    return BadRequest();
                }



                // 這裡 condition 也可以用 itemIDs.length
                // 因為執行到這邊已經保證 items 跟 itemIDs 長度一樣
                for(int i = 0; i < items.Length; i++)
                {
                    items[i].ConditionId = "P"; // Pending
                }

                // 每個 item 都要新增一筆 OrderDetail 的記錄
                OrderDetail[] details = new OrderDetail[items.Length];
                ItemLog[] logs = new ItemLog[items.Length];

                for(int i = 0; i < items.Length; i++)
                {
                    details[i] = new OrderDetail
                    {
                        OrderDetailId = Guid.NewGuid(),
                        OrderId = model.OrderID,
                        ItemId = items[i].ItemId,
                        OrderDetailStatusId = "P" // Pending
                    };

                    logs[i] = new ItemLog
                    {
                        ItemLogId = new Guid(),
                        OrderDetailId = details[i].OrderDetailId,
                        AdminId = adminId,
                        ItemId = details[i].ItemId,
                        ConditionId = "P"  // Pending
                    };
                }
                _dbContext.OrderDetails.AddRange(details);
                _dbContext.ItemLogs.AddRange(logs);

                order.OrderStatusId = "A"; // Approved
                response.Reply = "Y"; // Yes
            }
            else 
            {
                order.OrderStatusId = "D"; // Denied
                response.Reply = "N"; // No
            }

            _dbContext.Responses.Add(response);

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

        /*
         * OrderApi/CancelOrder
         */
        // 取消 Order
        [HttpPost]
        [Consumes("application/json")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(CancelOrderViewModel model)
        {
            Order order = await _dbContext.Orders
                .Where(o => o.OrderId == model.OrderID &&
                    o.OrderDetails.All(od => od.OrderDetailStatusId != "T")) // 此訂單的物品仍有東西應還能還但未還。
                .FirstOrDefaultAsync();


            if(order == null)
            {
                return NotFound();
            }


            OrderDetail[] details = await _dbContext.OrderDetails
                .Where(od => od.OrderId == order.OrderId)
                .ToArrayAsync();

            // 訂單改為取消狀態
            order.OrderStatusId = "C";

            // CanceledOrder 新增一筆紀錄
            CanceledOrder co = new CanceledOrder
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                Description = model.Description,
                CancelTime = DateTime.Now
            };
            _dbContext.CanceledOrders.Add(co);


            // 若 admin 已分配 item 給這筆 order
            // 還要再額外
            // 1. 把 item 的 condition 改回再庫（ItemLog 也要記錄 item 的改變）
            // 2. 把 order detail 的 status 改成取消
            if(details.Length != 0)
            {
                Guid[] itemIDs = details
                    .Select(od => od.ItemId)
                    .ToArray();

                Item[] items = await _dbContext.Items
                    .Where(i => itemIDs.Contains(i.ItemId))
                    .ToArrayAsync();

                foreach(OrderDetail detail in details)
                {
                    // item 的狀態改成入庫
                    Item item = items
                        .Where(i => i.ItemId == detail.ItemId)
                        .FirstOrDefault();
                    item.ConditionId = "I";

                    // order detail 的狀態改成取消
                    detail.OrderDetailStatusId = "C";


                    // ItemLog 新增一筆資料
                    ItemLog log = new ItemLog
                    {
                        ItemLogId = Guid.NewGuid(),
                        OrderDetailId = detail.OrderDetailId,
                        ItemId = detail.ItemId,
                        ConditionId = "I",
                        Description = model.Description,
                        CreateTime = DateTime.Now
                    };
                    if(User.HasClaim(ClaimTypes.Role, "admin"))
                    {
                        string adminIdString = User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                            .Value;
                        Guid adminId = Guid.Parse(adminIdString);

                        log.AdminId = adminId;
                    }

                    _dbContext.ItemLogs.Add(log);
                }
            }

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

        /*
         * OrderApi/CompleteOrder/{OrderID}
         */
        // 管理員確認訂單完成（該標記歸還的已標記歸還、該標記遺失的已標記遺失）
        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CompleteOrder(Guid id)
        {
            Order order = await _dbContext.Orders
                .Where(o => o.OrderId == id &&
                    o.OrderDetails.All(od => od.OrderDetailStatusId != "T"))
                .FirstOrDefaultAsync();

            if(order == null)
            {
                return NotFound();
            }

            order.OrderStatusId = "E"; // Ended

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return Conflict();
            }

            return Ok(id);
        }
    }
}
