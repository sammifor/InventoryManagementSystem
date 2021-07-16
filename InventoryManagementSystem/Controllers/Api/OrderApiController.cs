using InventoryManagementSystem.Models.EF;
using InventoryManagementSystem.Models.ResultModels;
using InventoryManagementSystem.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> MakeOrder(MakeOrderViewModel model)
        {
            if(model.EstimatedPickupTime < DateTime.Today)
            {
                return BadRequest();
            }

            Order order = new Order
            {
                UserId = model.UserId,
                EquipmentId = model.EquipmentId,
                Quantity = model.Quantity,
                EstimatedPickupTime = model.EstimatedPickupTime,
                Day = model.Day,

                // 前端沒權限給的
                OrderStatusId = "P",
                OrderTime = DateTime.Now
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

        /*
         * OrderApi/GetOrdersByUserId/{UserID}
         */
        // 以 UserID 查詢所有訂單（任何狀態的訂單都會查出來）
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<OrderResultModel[]> GetOrdersByUserId(int id)
        {
            var results = await _dbContext.Orders
                .Where(o => o.UserId == id)
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return results;
        }

        /*
         * OrderApi/GetPendingOrdersByUserId/{UserID}
         */
        // 以 UserID 查出待審核的 order
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<IActionResult> GetPendingOrdersByUserId(int id)
        {
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o => o.UserId == id &&
                    o.OrderStatusId == "P" && // Pending
                    o.EstimatedPickupTime > DateTime.Now) // 尚未過期的 order
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/GetReadyOrdersByUserId/{UserID}
         */
        // 以 UserID 查出待取貨的 order
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<IActionResult> GetReadyOrdersByUserId(int id)
        {
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o => o.UserId == id &&
                    o.OrderStatusId == "A" && // Order 是核可的
                    o.OrderDetails.Any(od => od.OrderDetailStatusId == "P") && // order 底下的 order detail 有待取貨的
                    o.EstimatedPickupTime.AddDays(o.Day) > DateTime.Now) // 沒過期的
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/GetOnGoingOrdersByUserId/{UserID}
         */
        // 以 UserID 查出租借中的 order
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<IActionResult> GetOnGoingOrdersByUserId(int id)
        {
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o => o.UserId == id &&
                    o.OrderStatusId == "A" && // 被核准的 order
                    o.OrderDetails.Any(od => od.OrderDetailStatusId == "T") && // 所訂的物品已被取貨
                    o.EstimatedPickupTime.AddDays(o.Day) > DateTime.Now) // 尚未逾期
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/GetClosedOrdersByUserId/{UserID}
         */
        // 以 UserID 查出已結束和管理員逾期回應的 order
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<IActionResult> GetClosedOrdersByUserId(int id)
        {
            string[] restrictions = { "C", "E", "D" }; // Canceled, ended and denied.
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o => o.UserId == id &&
                    (restrictions.Contains(o.OrderStatusId) || // 已取消、已結束、已拒絕
                        o.OrderStatusId == "P" && o.EstimatedPickupTime < DateTime.Now)) // 逾期回應
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/GetOverdueOrdersByUserId/{UserID}
         */
        // 以 UserID 查出已逾期且仍有物品尚未歸還的 order
        [HttpGet]
        [Produces("application/json")]
        [Route("{id}")]
        public async Task<IActionResult> GetOverdueOrdersByUserId(int id)
        {
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o => o.UserId == id &&
                    o.OrderStatusId == "A" &&
                    o.OrderDetails.Any(od => od.OrderDetailStatusId == "T") &&
                    o.EstimatedPickupTime.AddDays(o.Day) < DateTime.Now)
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/GetPendingOrders/
         */
        // 查出所有待審核的 order
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetPendingOrders()
        {
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o =>
                    o.OrderStatusId == "P" && // Pending
                    o.EstimatedPickupTime > DateTime.Now) // 尚未過期的 order
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/GetReadyOrders/
         */
        // 查出所有待取貨的 order
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetReadyOrders()
        {
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o =>
                    o.OrderStatusId == "A" && // Order 是核可的
                    o.OrderDetails.Any(od => od.OrderDetailStatusId == "P") && // order 底下的 order detail 有待取貨的
                    o.EstimatedPickupTime.AddDays(o.Day) > DateTime.Now) // 沒過期的
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/GetOnGoingOrders/
         */
        // 查出所有租借中的 order
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetOnGoingOrders()
        {
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o =>
                    o.OrderStatusId == "A" && // 被核准的 order
                    o.OrderDetails.Any(od => od.OrderDetailStatusId == "T") && // 所訂的物品已被取貨
                    o.EstimatedPickupTime.AddDays(o.Day) > DateTime.Now) // 尚未逾期
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/GetClosedOrders/
         */
        // 查出所有已結束和管理員逾期回應的 order
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetClosedOrders()
        {
            string[] restrictions = { "C", "E", "D" }; // Canceled, ended and denied.
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o =>
                    (restrictions.Contains(o.OrderStatusId) || // 已取消、已結束、已拒絕
                        o.OrderStatusId == "P" && o.EstimatedPickupTime < DateTime.Now)) // 逾期回應
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/GetOverDueOrders/
         */
        // 查出所有已逾期且仍有物品尚未歸還的 order
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetOverdueOrders()
        {
            OrderResultModel[] orders = await _dbContext.Orders
                .Where(o =>
                    o.OrderStatusId == "A" &&
                    o.OrderDetails.Any(od => od.OrderDetailStatusId == "T") &&
                    o.EstimatedPickupTime.AddDays(o.Day) < DateTime.Now)
                .Select(o => new OrderResultModel
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
                        OrderDetailStatus = od.OrderDetailStatus.StatusName
                    })
                        .ToArray()
                })
                .ToArrayAsync();

            return Ok(orders);
        }

        /*
         * OrderApi/RespondOrder
         */
        // Admin 核准或拒絕訂單
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> RespondOrder(RespondOrderViewModel model)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.OrderId == model.OrderID &&
                    o.OrderStatusId == "P" && // 待審核的 order
                    o.EstimatedPickupTime > DateTime.Now); // 尚未過期

            // 找不到訂單
            if (order == null)
            {
                return NotFound();
            }

            // 防止同個 item 被分配多次
            int[] itemIDs = model.ItemIDs.Distinct().ToArray();

            Item[] items = await _dbContext.Items
                .Where(i => itemIDs.Contains(i.ItemId))
                .ToArrayAsync();

            Response response = new Response
            {
                OrderId = model.OrderID,
                AdminId = 1 // TODO authentication
            };


            if(model.Reply == true)
            {
                // 訂單寫的數量與實際分配的數量不一致
                if (order.Quantity != itemIDs.Length)
                {
                    return BadRequest();
                }

                // 存在有分配的設備非訂單所寫的設備
                bool invalidEquipIdExists = items
                    .Any(i => i.EquipmentId != order.EquipmentId);

                if (invalidEquipIdExists)
                {
                    return BadRequest();
                }

                // 庫存不夠，無法滿足訂單
                int inStockNumber = await _dbContext.Items
                    .AsNoTracking()
                    .Where(i => i.EquipmentId == order.EquipmentId)
                    .CountAsync(i => i.ConditionId == "I");
                if (inStockNumber < order.Quantity)
                {
                    return BadRequest();
                }


                response.Reply = "Y";

                // 這裡 condition 也可以用 itemIDs.length
                // 因為執行到這邊已經保證 items 跟 itemIDs 長度一樣
                for(int i = 0; i < items.Length; i++)
                {
                    items[i].ConditionId = "P"; // Pending
                }

                // 每個 item 都要新增一筆 OrderDetail 的記錄
                OrderDetail[] details = new OrderDetail[items.Length];
                for(int i = 0; i < items.Length; i++)
                {
                    details[i] = new OrderDetail
                    {
                        OrderId = model.OrderID,
                        ItemId = items[i].ItemId,
                        OrderDetailStatusId = "P" // Pending
                    };

                }
                _dbContext.OrderDetails.AddRange(details);

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    return Conflict();
                }


                ItemLog[] logs = new ItemLog[items.Length];
                for(int i = 0; i < items.Length; i++)
                {
                    logs[i] = new ItemLog
                    {
                        OrderDetailId = details[i].OrderDetailId,
                        AdminId = 1, // TODO authentication
                        ItemId = details[i].ItemId,
                        ConditionId = "P"  // Pending
                    };
                }
                _dbContext.ItemLogs.AddRange(logs);

                order.OrderStatusId = "A"; // Approved
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
        public async Task<IActionResult> CancelOrder(CancelOrderViewModel model)
        {
            Order order = await _dbContext.Orders
                .Where(o => o.OrderId == model.OrderID &&
                    o.OrderDetails.Any(od => od.OrderDetailStatusId == "T")) // 此訂單的物品仍有東西應還能還但未還。
                .FirstOrDefaultAsync();


            if (order == null)
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
            if (details.Length != 0)
            {
                int[] itemIDs = details
                    .Select(od => od.ItemId)
                    .ToArray();

                Item[] items = await _dbContext.Items
                    .Where(i => itemIDs.Contains(i.ItemId))
                    .ToArrayAsync();

                foreach (OrderDetail detail in details)
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
                        OrderDetailId = detail.OrderDetailId,
                        AdminId = 1, //TODO authentication
                        ItemId = detail.ItemId,
                        ConditionId = "I",
                        Description = model.Description,
                        CreateTime = DateTime.Now
                    };

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
        public async Task<IActionResult> CompleteOrder(int id)
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
