using MFilesImporter.Model;
using MFilesImporter.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Test.Service
{
  [TestClass]
  public class ConnectionTestServiceTest
  {
    private IConnectionTestService GetService()
    {
      var settingsService = new Mock<ISettingsService>();
      settingsService.Setup(m => m.Settings).Returns(new SettingsModel());

      var parametersService = new Mock<IParametersService>();
      parametersService.Setup(m => m.Parameters).Returns(new ParametersModel()
      {
        User = "importer",
        Password = "importer",
        Vault = "Sample Vault"
      });

      var vaultService = new VaultService(settingsService.Object, parametersService.Object);
      var connectionTestService = new ConnectionTestService(vaultService);
      return connectionTestService;
    }

    [TestMethod]
    public void ConnectionTestServiceTest_Ctor()
    {
      GetService();
    }

    [TestMethod]
    public void ConnectionTestServiceTest_TestConnection()
    {
      var service = GetService();
      Assert.IsTrue(service.TestConnection());
    }
  }
}
