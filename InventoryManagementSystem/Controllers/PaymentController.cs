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

    }
}
