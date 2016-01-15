using MFilesImporter.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Test
{
  [TestClass]
  public class SettingsFactoryTest
  {
    private ISettingsFactory GetFactory()
    {
      return new SettingsFactory();
    }

    [TestMethod]
    public void SettingsFactoryTest_GetSettings()
    {
      var factory = GetFactory();
      var settings = factory.GetSettings();

      Assert.AreEqual<string>("importer", settings.User);
      Assert.AreEqual<string>("importer", settings.Password);
      Assert.AreEqual<string>("Sample Vault", settings.Vault);
    }
  }
}
