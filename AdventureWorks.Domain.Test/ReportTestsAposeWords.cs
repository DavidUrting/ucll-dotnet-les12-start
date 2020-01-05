using Aspose.Words;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace AdventureWorks.Domain.Test
{
    [TestClass]
    public class ReportTestsAposeWords
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
            string outputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");

            try
            {
                // ACT
                Document doc = new Document();
                DocumentBuilder docBuilder = new DocumentBuilder(doc);
                Paragraph p = docBuilder.InsertParagraph();
                p.Runs.Add(new Run(doc, "Testrapport Aspose.Words"));
                docBuilder.StartTable();
                docBuilder.InsertCell();
                docBuilder.Write("ID");
                docBuilder.InsertCell();
                docBuilder.Write("Description");
                docBuilder.EndRow();
                foreach (var item in testData)
                {
                    docBuilder.InsertCell();
                    docBuilder.Write(item.Item1.ToString());
                    docBuilder.InsertCell();
                    docBuilder.Write(item.Item2);
                    docBuilder.EndRow();
                }
                docBuilder.EndTable();
                doc.Save(outputPath, SaveFormat.Pdf);

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