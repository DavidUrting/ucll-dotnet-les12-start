using AdventureWorks.Domain.Models;
using System.Collections.Generic;
using System.IO;

namespace AdventureWorks.Domain
{
    public interface IProductManager
    {
        Stream GenerateAllProductsReport();
    }
}
