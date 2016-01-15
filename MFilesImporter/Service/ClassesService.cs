using MFilesAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface IClassesService
  {
    IEnumerable<string> GetClasses();
  }

  class ClassesService : IClassesService
  {
    private readonly IVaultService VaultService;

    public ClassesService(IVaultService vaultService)
    {
      this.VaultService = vaultService;
    }

    public IEnumerable<string> GetClasses()
    {
      var vault = this.VaultService.Vault.Value;

      var objectClasses = vault.ClassOperations.GetAllObjectClasses();

      foreach (ObjectClass objectClass in objectClasses)
        yield return objectClass.Name;
    }
  }
}
