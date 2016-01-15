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
  public class DestroyObjectServiceTest
  {
    private IDestroyObjectService GetService()
    {
      var vaultService = VaultServiceTest.GetService();
      var findObjectsService = new FindObjectsService(vaultService);
      return new DestroyObjectService(vaultService, findObjectsService);
    }

    [TestMethod]
    public void DestroyObjectServiceTest_Ctor()
    {
      GetService();
    }

    [TestMethod, Timeout(60*60*60)]
    public void DestroyObjectServiceTest_Destroy_RSIs()
    {
      var service = GetService();

      service.DestroyObjectsByClass("RSI");
    }
  }
}
