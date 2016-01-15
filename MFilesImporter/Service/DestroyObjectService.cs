using MFilesAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  /// <summary>Destroys objects based on properties</summary>
  interface IDestroyObjectService
  {
    int DestroyObjectsByClass(string className);
    int DestroyObjectsByObjectType(string objectType);
  }

  class DestroyObjectService : IDestroyObjectService
  {
    private readonly IVaultService VaultService;
    private readonly IFindObjectsService FindObjectService;

    public DestroyObjectService(IVaultService vaultService,
                                IFindObjectsService findObjectsService)
    {
      this.VaultService = vaultService;
      this.FindObjectService = findObjectsService;
    }

    private void DestroyObject(ObjectVersion objectVersion)
    {
      var vault = this.VaultService.Vault.Value;
      vault.ObjectOperations.DestroyObject(objectVersion.ObjVer.ObjID, true, -1);
    }

    private int DestroySearchResults(ObjectSearchResults objectSearchResults)
    {
      foreach (ObjectVersion objectVersion in objectSearchResults)
        DestroyObject(objectVersion);

      return objectSearchResults.Count;
    }

    public int DestroyObjectsByClass(string className)
    {
      return DestroySearchResults(this.FindObjectService.FindObjectsByClass(className));
    }

    public int DestroyObjectsByObjectType(string objectType)
    {
      return DestroySearchResults(this.FindObjectService.FindObjectsByObjectType(objectType));
    }

  }
}
