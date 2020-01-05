using Microsoft.VisualStudio.TestTools.UnitTesting;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventureWorks.Domain.Test
{
    [TestClass]
    public class ReportTestsPdfSharpAndMigraDoc
    {
        [TestMethod]
        public void TestRepeatingData()
        {
            // ARRANGE
            List<Tuple<int, string>> testData = new List<Tuple<int, string>>()
            {
                new Tuple<int, string>(1, "Item 1"),
                new Tuple<int, string>(2, "Item 2")
            };
            // Hiervan is PdfSharp/MigraDoc afhankelijk ... 
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();
            string outputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");

            try
            {
                // ACT
                Document doc = new Document();
                Section mainSection = doc.AddSection();
                mainSection.AddParagraph("Testrapport");
                Table table = mainSection.AddTable();
                Column colID = table.AddColumn(new Unit(100));
                Column colDescription = table.AddColumn(new Unit(500));
                Row header = table.AddRow();
                header[0].AddParagraph("ID");
                header[1].AddParagraph("Description");
                foreach (var item in testData)
                {
                    Row row = table.AddRow();
                    row[0].AddParagraph(item.Item1.ToString());
                    row[1].AddParagraph(item.Item2);
                }
                pdfRenderer.Document = doc;
                pdfRenderer.RenderDocument();
                pdfRenderer.PdfDocument.Save(outputPath);

                // ASSERT
                Assert.IsTrue(File.Exists(outputPath));
            }
            finally
            {
                if (File.Exists(outputPath)) File.Delete(outputPath);
            }
        }
    }
}