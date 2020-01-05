using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorks.Web.Areas.Production.Controllers;
using AdventureWorks.Web.Areas.Sales.Controllers;
using AdventureWorks.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdventureWorks.Web.Controllers
{
    public class ReportController : Controller
    {
        private const string ALL_PRODUCTS = "AP";
        private const string ALL_CUSTOMERS = "AC";

        public IActionResult Index()
        {
            ReportViewModel vm = new ReportViewModel();

            vm.AvailableReports.Add(new SelectListItem("All products", ALL_PRODUCTS));
            vm.AvailableReports.Add(new SelectListItem("All customers", ALL_CUSTOMERS));

            return View(vm);
        }

        [HttpPost]
        public IActionResult ShowReport(string report)
        {
            switch (report)
            {
                case ALL_PRODUCTS:
                    return RedirectToAction(
                        nameof(ProductController.GetAllProductsReport),
                        nameof(ProductController).Replace("Controller", ""),
                        new { area = "Production" });
                case ALL_CUSTOMERS:
                    return RedirectToAction(
                        nameof(CustomerController.GetAllCustomersReport), 
                        nameof(CustomerController).Replace("Controller", ""), 
                        new { area = "Sales" });
                default:
                    return NotFound();
            }
        }
    }
}