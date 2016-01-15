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
  public class ObjectTypeServiceTest
  {
    private IObjectTypeService GetService()
    {
      return new ObjectTypeService(VaultServiceTest.GetService());
    }

    [TestMethod]
    public void ObjectTypeServiceTest_Ctor()
    {
      var service = GetService();
    }

    [TestMethod]
    public void ObjectTypeServiceTest_GetObjectTypes()
    {
      var service = GetService();
      var objectTypes = service.GetObjectTypes();
      foreach (var objectType in objectTypes)
        Console.WriteLine(objectType);
    }
  }
}
