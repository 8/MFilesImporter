using MFilesImporter.Model;
using MFilesImporter.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
  public class CsvWriterServiceTest
  {
    private ICsvWriterService GetService()
    {
      return new CsvWriterService();
    }

    [TestMethod]
    public void CsvWriterServiceTest_Ctor()
    {
      GetService();
    }

    [TestMethod]
    public void CsvWriterServiceTest_Write()
    {
      var csvModel = new CsvModel();
      var service = GetService();

      using (var sw = new StringWriter())
      {
        service.Write(new [] { csvModel }, sw);
        Debug.Write(sw.ToString());
      }
    }

    [TestMethod]
    public void CsvWriterServiceTest_Write_CsvModels()
    {
      var models = new CsvParserService().GetCsvModels(TestData.GetTestFile("Unclassified Document.csv")).ToArray();

      var service = GetService();

      using (var sw = new StringWriter())
      {
        service.Write(models, sw);
        Debug.Write(sw.ToString());
      }
    }
  }
}
