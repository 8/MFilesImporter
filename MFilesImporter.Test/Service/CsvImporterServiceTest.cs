using MFilesImporter.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Test.Service
{
  [TestClass]
  public class CsvImporterServiceTest
  {
    ICsvImporterService GetService()
    {
      return new CsvImporterService(VaultServiceTest.GetService());
    }

    [TestMethod]
    public void CsvImporterServiceTest_Ctor()
    {
      GetService();
    }

    [TestMethod]
    public void CsvImporterServiceTest_Import_Unclassified_Test()
    {
      string file = TestData.GetTestFile("Unclassified Document.csv");
      Assert.IsTrue(File.Exists(file));

      var csvParserService = new CsvParserService();
      var csvModels = csvParserService.GetCsvModels(file).ToArray();

      var service = GetService();
      foreach (var csvModel in csvModels)
      {
        Debug.Write(csvModel);

        service.Import(csvModel);
      }
    }

    private void ImportFromFile(string fileName)
    {
      string file = TestData.GetTestFile(fileName);
      Assert.IsTrue(File.Exists(file));

      var csvParserService = new CsvParserService();
      var csvModels = csvParserService.GetCsvModels(file).ToArray();
      var service = GetService();

      foreach (var csvModel in csvModels)
        service.Import(csvModel);
    }

  }
}
