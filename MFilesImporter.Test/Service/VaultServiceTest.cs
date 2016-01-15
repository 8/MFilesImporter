using MFilesImporter.Model;
using MFilesImporter.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Test.Service
{
  [TestClass]
  public class VaultServiceTest
  {
    internal static IVaultService GetService()
    {
      var settingsService = new Mock<ISettingsService>();
      settingsService.Setup(m => m.Settings).Returns(new SettingsModel());

      var parametersService = new Mock<IParametersService>();
      parametersService.Setup(m => m.Parameters).Returns(new ParametersModel()
      {
        User = "importer",
        Password = "importer",
        Vault = "Sample Vault"
        //Vault = "My Vault"
      });

      return new VaultService(settingsService.Object, parametersService.Object);
    }

    [TestMethod]
    public void VaultServiceTest_Ctor()
    {
      GetService();
    }

    [TestMethod]
    public void VaultServiceTest_Vault()
    {
      var service = GetService();
    }
  }
}
