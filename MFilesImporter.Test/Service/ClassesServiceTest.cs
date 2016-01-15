using MFilesImporter.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Test.Service
{
  [TestClass]
  public class ClassesServiceTest
  {
    private IClassesService GetService()
    {
      return new ClassesService(VaultServiceTest.GetService());
    }

    [TestMethod]
    public void ClassesServiceTest_Ctor()
    {
      GetService();
    }

    [TestMethod]
    public void ClassesServiceTest_GetClasses()
    {
      var service = GetService();
      var classes = service.GetClasses();

      foreach (var c in classes)
        Debug.WriteLine(c);
    }
  }
}
