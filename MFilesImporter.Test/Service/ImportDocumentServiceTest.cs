//using MFilesImporter.Model;
//using MFilesImporter.Service;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MFilesImporter.Test.Service
//{
//  [TestClass]
//  public class ImportDocumentServiceTest
//  {
//    IImportDocumentService GetService()
//    {
//      return new ImportDocumentService(VaultServiceTest.GetService());
//    }

//    [TestMethod]
//    public void ImportDocumentServiceTest_Ctor()
//    {
//      GetService();
//    }

//    [TestMethod]
//    public void ImportDocumentServiceTest_Import()
//    {
//      var service = GetService();

//      var document = new DocumentModel() {
//        Datum = DateTime.Today
//      };

//      document.SourceFiles.Add(new SourceFileModel()
//      {
//        Title = "ImportTest",
//        FilePath = @"C:\Import\DemoTest.txt",
//        Extension = "txt"
//      });

//      service.Import(document);
//    }
//  }
//}
