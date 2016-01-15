using MFilesAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface IObjectTypeService
  {
    IEnumerable<string> GetObjectTypes();
  }

  class ObjectTypeService : IObjectTypeService
  {
    private readonly IVaultService VaultService;

    public ObjectTypeService(IVaultService vaultService)
    {
      this.VaultService = vaultService;
    }

    public IEnumerable<string> GetObjectTypes()
    {
      var vault = this.VaultService.Vault.Value;

      var objTypes = vault.ObjectTypeOperations.GetObjectTypes();

      foreach (ObjType objType in objTypes)
      {
        yield return objType.NameSingular;
      }
    }
  }
}
