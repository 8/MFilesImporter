using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface IConnectionTestService
  {
    bool TestConnection();
  }

  class ConnectionTestService : IConnectionTestService
  {
    private readonly IVaultService VaultService;

    public ConnectionTestService(IVaultService vaultService)
    {
      this.VaultService = vaultService;
    }

    public bool TestConnection()
    {
      var vault = VaultService.Vault.Value;
      if (vault != null)
      {
        string vaultName = vault.Name;
        //Console.WriteLine("Vault: " + vaultName);
        return vault.LoggedIn;
      }
      else
        return false;
    }
  }
}
