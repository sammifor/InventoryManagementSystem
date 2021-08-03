using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models.ViewModels
{
    public class CloseReportViewModel
    {
        public Guid ReportId { get; set; }
        public string Note { get; set; }
    }
}
