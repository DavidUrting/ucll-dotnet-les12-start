using Aspose.Words;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace AdventureWorks.Domain
{
    public class ProductManager : IProductManager
    {
        private string _connString;
        public ProductManager(string connString)
        {
            _connString = connString;
        }
        public Stream GenerateAllProductsReport()
        {
            using (var dbContext = new Entities.AdventureWorks2017Context(_connString))
            {
                MemoryStream memStream = new MemoryStream();

                // Opgelet: hier wordt er rechtstreeks op de entiteiten gewerkt
                // en niet op een domain objects.
                // Het gebruik van domain objects is niet nodig omdat ProductManager 
                // enkel rapporten kan aanmaken en dus geen Search, Get, Insert, ... methods heeft.
                var products = dbContext.Product
                    .Include(p => p.ProductSubcategory)
                    .Where(p => p.ProductSubcategory != null)
                    .OrderBy(p => p.ProductSubcategory.Name)
                    .ThenBy(p => p.Name)
                    .ToList();

                Document doc = new Document();
                DocumentBuilder docBuilder = new DocumentBuilder(doc);
                Paragraph p = docBuilder.InsertParagraph();
                p.Runs.Add(new Run(doc, "AdventureWorks Products"));
                docBuilder.StartTable();
                docBuilder.InsertCell();
                docBuilder.Write("ID");
                docBuilder.InsertCell();
                docBuilder.Write("Product");
                docBuilder.InsertCell();
                docBuilder.Write("Subcategory");
                docBuilder.EndRow();
                foreach (var product in products)
                {
                    docBuilder.InsertCell();
                    docBuilder.Write(product.ProductId.ToString());
                    docBuilder.InsertCell();
                    docBuilder.Write(product.Name);
                    docBuilder.InsertCell();
                    docBuilder.Write(product.ProductSubcategory.Name);
                    docBuilder.EndRow();
                }
                docBuilder.EndTable();

                // Document wordt nu bewaard in een memorystream
                // (= er wordt geen bestand op het file system aangemaakt).
                doc.Save(memStream, SaveFormat.Pdf);

                // stream pointer terug aan het begin plaatsen!
                memStream.Position = 0;

                return memStream;    
            }
        }
    }
}
