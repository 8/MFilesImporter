using MFilesAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface IFindObjectsService
  {
    ObjectSearchResults FindObjectsByClass(string className);
    ObjectSearchResults FindObjectsByObjectType(string objectType);
  }

  class FindObjectsService : IFindObjectsService
  {
    private readonly IVaultService VaultService;

    public FindObjectsService(IVaultService vaultService)
    {
      this.VaultService = vaultService;
    }

    #region Methods

    private int GetClassId(string className)
    {
      int classId = -1;
      var vault = this.VaultService.Vault.Value;
      var objectClasses = vault.ClassOperations.GetAllObjectClasses();
      foreach (ObjectClass objectClass in objectClasses)
      {
        if (objectClass.Name == className)
        {
          classId = objectClass.ID;
          break;
        }
      }
      return classId;
    }

    private int GetObjectTypeId(string objectType)
    {
      int objectTypeId = -1;
      var vault = this.VaultService.Vault.Value;
      var objTypes = vault.ObjectTypeOperations.GetObjectTypes();
      foreach (ObjType objType in objTypes)
      {
        if (objectType == objType.NameSingular)
        {
          objectTypeId = objType.ID;
          break;
        }
      }
      return objectTypeId;
    }

    private ObjectSearchResults Find(SearchCondition searchCondition)
    {
      var vault = this.VaultService.Vault.Value;

      var searchConditions = new SearchConditions();
      searchConditions.Add(-1, searchCondition);

      var objectSearchResults = vault.ObjectSearchOperations.SearchForObjectsByConditionsEx(
        searchConditions,
        MFSearchFlags.MFSearchFlagLookInAllVersions | MFSearchFlags.MFSearchFlagDisableRelevancyRanking,
        false,
        MaxResultCount: 100000,
        SearchTimeoutInSeconds: Int32.MaxValue);

      return objectSearchResults;
    }

    public ObjectSearchResults FindObjectsByObjectType(string objectType)
    {
      var vault = this.VaultService.Vault.Value;

      int objectTypeId = GetObjectTypeId(objectType);

      /* find all files with the specified object type */
      var searchCondition = new SearchCondition();
      searchCondition.ConditionType = MFConditionType.MFConditionTypeEqual;
      searchCondition.Expression.DataStatusValueType = MFStatusType.MFStatusTypeObjectTypeID;
      searchCondition.TypedValue.SetValue(MFDataType.MFDatatypeLookup, objectTypeId);

      return Find(searchCondition);
    }

    public ObjectSearchResults FindObjectsByClass(string className)
    {
      var vault = this.VaultService.Vault.Value;

      int classId = GetClassId(className);

      /* find all files with the specified class */
      var searchCondition = new SearchCondition();
      searchCondition.ConditionType = MFConditionType.MFConditionTypeEqual;
      searchCondition.Expression.DataPropertyValuePropertyDef = (int)MFBuiltInPropertyDef.MFBuiltInPropertyDefClass;
      searchCondition.TypedValue.SetValue(MFDataType.MFDatatypeLookup, classId);

      return Find(searchCondition);
    }

    #endregion
  }
}
