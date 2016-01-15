using MFilesImporter.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Test.Service
{
  [TestClass]
  public class CsvParserServiceTest
  {
    private ICsvParserService GetService()
    {
      return new CsvParserService();
    }

    [TestMethod]
    public void CsvParserServiceTest_Ctor()
    {
      GetService();
    }

    [TestMethod]
    public void CsvParserServiceTest_Create()
    {
      var service = GetService();
      var models = service.GetCsvModels(TestData.GetTestFile("Unclassified Document.csv")).ToArray();

      Assert.AreEqual<int>(2, models.Length);
      var model = models[0];

      Assert.AreEqual<string>("Document", model.ObjectType);
      Assert.AreEqual<string>("Unclassified Document", model.Class);

      Console.WriteLine("Values:");
      foreach (var kv in model.Values)
        Console.WriteLine(kv);
    }
  }
}
