using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.Web.Models
{
    public class ReportViewModel
    {
        public IList<SelectListItem> AvailableReports { get; set; } = new List<SelectListItem>();

        public string SelectedReport { get; set; }
    }
}
