using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdventureWorks.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.Web.Areas.Production.Controllers
{
    public class ProductController : Controller
    {
        private IProductManager _manager;

        public ProductController(IProductManager manager)
        {
            _manager = manager;
        }

        public IActionResult GetAllProductsReport()
        {
            Stream reportStream = _manager.GenerateAllProductsReport();
            return File(
                reportStream,
                "application/pdf");
        }
    }
}