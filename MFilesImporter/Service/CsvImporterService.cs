using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using CsvHelper;
using MFilesImporter.Model;
using MFilesAPI;
using System.Diagnostics;

namespace MFilesImporter.Service
{
  /// <summary>Imports a CsvModel into M-Files</summary>
  interface ICsvImporterService
  {
    int Import(CsvModel csvModel);
  }

  class CsvImporterService : ICsvImporterService
  {
    private readonly IVaultService VaultService;

    private Dictionary<string, ObjectClass> ObjectClassLookup = null;
    private Dictionary<string, ObjType> ObjectTypeLookup = null;
    private Dictionary<string, PropertyDef> PropertyDefLookupByName = null;
    private Dictionary<int, PropertyDef> PropertyDefLookupById = null;

    public CsvImporterService(IVaultService vaultService)
    {
      this.VaultService = vaultService;
    }

    #region Methods

    private int GetPropertyDefId(string propertyName, Vault vault)
    {
      var propertyDefs = vault.PropertyDefOperations.GetPropertyDefs();
      int propertyDefId = -1;

      foreach (PropertyDef propertyDef in propertyDefs)
        if (propertyDef.Name == propertyName)
        {
          propertyDefId = propertyDef.ID;
          break;
        }
      return propertyDefId;
    }

    private void LogPropertyDef(PropertyDef propertyDef)
    {
      Debug.WriteLine("Name: " + propertyDef.Name);
      Debug.WriteLine("ContentType: " + propertyDef.ContentType);
      Debug.WriteLine("ObjectType: " + propertyDef.ObjectType);
      Debug.WriteLine("DependencyPD: " + propertyDef.DependencyPD);
      Debug.WriteLine("DependencyRelation: " + propertyDef.DependencyRelation);
      Debug.WriteLine("ValueList: " + propertyDef.ValueList);
      Debug.WriteLine("StaticFilter: " + propertyDef.StaticFilter.Count);

      foreach (SearchCondition condition in propertyDef.StaticFilter)
        Debug.WriteLine("condition.ConditionType: " + condition.ConditionType);
      Debug.WriteLine("");
    }

    //private int GetValueListItemId(int valueListId, string value, Vault vault, SearchConditions searchConditions)
    private int GetValueListItemId(PropertyDef propertyDef, string value, Vault vault)
    {
      int valueListItemId = -1;

      System.Collections.IEnumerable items = null;

      bool useSearch = false;

      /* search fails for MFExpressionTypePropertyValue */
      foreach (SearchCondition searchCondition in propertyDef.StaticFilter)
      {
        Debug.WriteLine("searchCondition: " + searchCondition.Expression.Type.ToString() + " " +
                        "condition: " + searchCondition.ConditionType + " " +
                        "typedValue: " + searchCondition.TypedValue.DisplayValue);
        useSearch = searchCondition.Expression.Type == MFExpressionType.MFExpressionTypeTypedValue;
      }

      /* use the search trim down elements (value list items are capped at 5000) */
      if (useSearch)
        items = vault.ValueListItemOperations.SearchForValueListItemsEx(propertyDef.ValueList, propertyDef.StaticFilter);
      else
        items = vault.ValueListItemOperations.GetValueListItems(propertyDef.ValueList);

      //int index = 0;
      foreach (ValueListItem valueListItem in items)
      {
        if (valueListItem.Name == value)
        {
          valueListItemId = valueListItem.ID;
          break;
        }
        //index++;
      }

      //Debug.WriteLine(index);

      Debug.WriteLine(string.Format("valueListId={0}, value={1} => valueListItemId={2}", propertyDef.ValueList, value, valueListItemId));
      return valueListItemId;
    }

    private object ConvertPropertyValue(PropertyDef propertyDef, string value, Vault vault)
    {
      object ret;
      switch (propertyDef.DataType)
      {
        case MFDataType.MFDatatypeText:
          ret = value;
          break;

        case MFDataType.MFDatatypeDate:
          DateTime dt;
          if (DateTime.TryParse(value, out dt))
            ret = dt;
          else
            throw new ArgumentException(string.Format("failed to convert '{0}' to a valid date", value));
          break;

        case MFDataType.MFDatatypeLookup:
          /* we need to retrieve the id of the lookup */
          {
            LogPropertyDef(propertyDef);

            //Debug.WriteLine("based on valueList: " + propertyDef.BasedOnValueList + " " + 
            //  propertyDef.StaticFilter.);
            //int valueListItemId = GetValueListItemId(propertyDef.ValueList, value, vault, propertyDef.StaticFilter);
            int valueListItemId = GetValueListItemId(propertyDef, value, vault);
            ret = valueListItemId;
            if (valueListItemId == -1)
              throw new ArgumentException(string.Format("failed to find valuelist entry '{0}' for property '{1}'", value, propertyDef.Name));
          }
          break;

        case MFDataType.MFDatatypeMultiSelectLookup:
          {
            /* split into multiple values */
            string[] values = value.Split(Constants.MultiSelectDelimiter);
            int[] valueListItemIds = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
              //valueListItemIds[i] = GetValueListItemId(propertyDef.ValueList, value, vault, propertyDef.StaticFilter);
              valueListItemIds[i] = GetValueListItemId(propertyDef, value, vault);
              if (valueListItemIds[i] == -1)
                throw new ArgumentException(string.Format("failed to find valuelist entry '{0}' for property '{1}'", value, propertyDef.Name));
            }
            ret = valueListItemIds;
          }
          break;

        case MFDataType.MFDatatypeBoolean:
          if (value.ToLower() == "yes" ||
              value.ToLower() == "ja")
            ret = true;
          else
            ret = false;
          break;

        case MFDataType.MFDatatypeInteger:
          {
            int i;
            if (Int32.TryParse(value, out i))
              ret = i;
            else
              throw new ArgumentException(string.Format("failed to convert value '{0}' to an integer for property '{1}'", value, propertyDef.Name));
          }
          break;

        default:
          LogPropertyDef(propertyDef);
          throw new NotImplementedException(string.Format("converting to '{0}' is not implemented.", propertyDef.DataType));
      }

      return ret;
    }

    private PropertyDef GetPropertyDefByName(string name, Vault vault)
    {
      PropertyDef ret = null;
      var propertyDefs = vault.PropertyDefOperations.GetPropertyDefs();
      foreach (PropertyDef propertyDef in propertyDefs)
      {
        if (propertyDef.Name == name)
        {
          ret = propertyDef;
          break;
        }
      }
      return ret;
    }

    private PropertyValues GetPropertyValues(CsvModel csvModel, 
      Vault vault,
      int classId,
      ObjType objType,
      Dictionary<string, ObjType> objectTypeLookup,
      Dictionary<string, PropertyDef> propertyDefLookupByName,
      Dictionary<int, PropertyDef> propertyDefLookupById)
    {
      var propertyValues = new MFilesAPI.PropertyValues();

      int index = 0;

      /* add the special class property */
      propertyValues.Add(index++, GetPropertyValue((int)MFilesAPI.MFBuiltInPropertyDef.MFBuiltInPropertyDefClass, MFDataType.MFDatatypeLookup, classId));

      ObjectClass objectClass = vault.ClassOperations.GetObjectClass(classId);

      /* if the object type has an owner */
      if (objType.HasOwnerType)
      {
        /* add a special property for the owner */
        var ownerObjType = vault.ObjectTypeOperations.GetObjectType(objType.OwnerType);
        var propertyDef = propertyDefLookupByName[ownerObjType.NameSingular];

        string value;
        if (csvModel.Values.TryGetValue(propertyDef.Name, out value))
        {
          object objValue = ConvertPropertyValue(propertyDef, value, vault);
          var propertyValue = GetPropertyValue(propertyDef.ID, propertyDef.DataType, objValue);
          propertyValues.Add(index++, propertyValue);
        }

      }

      foreach (AssociatedPropertyDef associatedPropertyDef in objectClass.AssociatedPropertyDefs)
      {
        /* get the property definition id */
        PropertyDef propertyDef = PropertyDefLookupById[associatedPropertyDef.PropertyDef];

        string value;
        if (csvModel.Values.TryGetValue(propertyDef.Name, out value))
        {
          /* try to convert the string to the ObjectData Type */
          object objValue = ConvertPropertyValue(propertyDef, value, vault);

          /* create the property value */
          PropertyValue propertyValue = GetPropertyValue(associatedPropertyDef.PropertyDef, propertyDef.DataType, objValue);

          Debug.WriteLine("property - Id: {0}, Name: {1}, DataType: {2}, Value: {3}", associatedPropertyDef.PropertyDef, propertyDef.Name, propertyDef.DataType, objValue);

          /* add the property value */
          propertyValues.Add(index++, propertyValue);

        }
      }

      return propertyValues;
    }

    private Dictionary<string, ObjType> GetObjTypeLookup(Vault vault)
    {
      var lookup = new Dictionary<string, ObjType>();

      ObjTypes objectTypes = vault.ObjectTypeOperations.GetObjectTypes();

      foreach (ObjType objectType in objectTypes)
      {
        if (!lookup.ContainsKey(objectType.NameSingular))
          lookup.Add(objectType.NameSingular, objectType);
      }

      return lookup;
    }

    private Dictionary<string, ObjectClass> GetObjectClassLookup(Vault vault)
    {
      var lookup = new Dictionary<string, ObjectClass>();
      var allObjectClasses = vault.ClassOperations.GetAllObjectClasses();
      foreach (ObjectClass objectClass in allObjectClasses)
      {
        if (!lookup.ContainsKey(objectClass.Name))
          lookup.Add(objectClass.Name, objectClass);
      }
      return lookup;
    }

    private Dictionary<string, PropertyDef> GetPropertyDefLookupByName(Vault vault)
    {
      var lookup = new Dictionary<string, PropertyDef>();
      var propertyDefs = vault.PropertyDefOperations.GetPropertyDefs();
      foreach (PropertyDef propertyDef in propertyDefs)
      {
        if (!lookup.ContainsKey(propertyDef.Name))
          lookup.Add(propertyDef.Name, propertyDef);
      }
      return lookup;
    }

    private Dictionary<int, PropertyDef> GetPropertyDefLookupById(Vault vault)
    {
      var lookup = new Dictionary<int, PropertyDef>();
      var propertyDefs = vault.PropertyDefOperations.GetPropertyDefs();
      foreach (PropertyDef propertyDef in propertyDefs)
      {
        if (!lookup.ContainsKey(propertyDef.ID))
          lookup.Add(propertyDef.ID, propertyDef);
      }
      return lookup;
    }

    //private int GetObjectTypeId(CsvModel csvModel, Vault vault)
    //{
    //  ObjTypes objectTypes = vault.ObjectTypeOperations.GetObjectTypes();
    //  int objectTypeId = -1;

    //  foreach (ObjType objectType in objectTypes)
    //  {
    //    if (objectType.NameSingular == csvModel.ObjectType)
    //    {
    //      objectTypeId = objectType.ID;
    //      break;
    //    }

    //    Debug.WriteLine(objectType.NameSingular);
    //  }
    //  return objectTypeId;
    //}

    //private int GetClassId(CsvModel csvModel, Vault vault)
    //{
    //  var allObjectClasses = vault.ClassOperations.GetAllObjectClasses();
    //  int classId = -1;
    //  foreach (ObjectClass objectClass in allObjectClasses)
    //  {
    //    if (csvModel.Class == objectClass.Name)
    //    {
    //      classId = objectClass.ID;
    //      break;
    //    }
    //    //Debug.WriteLine(string.Format("Id={0}, Name={1}", objectClass.ID, objectClass.Name));
    //  }
    //  return classId;
    //}

    //private ObjectVersionAndProperties GetObjectVersionAndPropertiesAndCheckin(CsvModel csvModel, Vault vault, PropertyValues propertyValues, SourceObjectFiles sourceObjectFiles, int objectTypeId)
    //{
    //  if (objectTypeId == -1)
    //    throw new ArgumentException(string.Format("could not find objectType with name '{0}'", csvModel.ObjectType));

    //  Debug.WriteLine("objectTypeId: " + objectTypeId);

    //  return vault.ObjectOperations.CreateNewObjectEx(objectTypeId, propertyValues, SourceFiles, false, false)
    //  return vault.ObjectOperations.CreateNewObject(objectTypeId, propertyValues, sourceObjectFiles);
    //}

    private ObjectVersionAndProperties GetObjectVersionAndProperties(CsvModel csvModel, Vault vault, PropertyValues propertyValues, SourceObjectFiles sourceObjectFiles, int objectTypeId)
    {
      if (objectTypeId == -1)
        throw new ArgumentException(string.Format("could not find objectType with name '{0}'", csvModel.ObjectType));

      Debug.WriteLine("objectTypeId: " + objectTypeId);

      //return vault.ObjectOperations.CreateNewObjectEx(objectTypeId, propertyValues, sourceObjectFiles, false, false);
      return vault.ObjectOperations.CreateNewObject(objectTypeId, propertyValues, sourceObjectFiles);
    }

    private PropertyValue GetPropertyValue(int propertyDef, MFDataType dataType, object value)
    {
      var propertyValue = new PropertyValue();
      propertyValue.PropertyDef = propertyDef;
      try { propertyValue.TypedValue.SetValue(dataType, value); }
      catch (Exception ex)
      {
        throw new ArgumentException(
          string.Format("propertyValue.TypedValue.SetValue() failed with parameters: propertyDefId: {0}, dataType: {1}, value: {2}",
          propertyDef, dataType, value), ex);
      }
      return propertyValue;
    }

    private SourceObjectFiles GetSourceObjectFiles(CsvModel csvModel)
    {
      SourceObjectFiles sourceObjectFiles = null;

      var sourceFiles = csvModel.SourceFiles.Select(f => SourceFileModel.FromFile(f)).ToArray();

      Debug.WriteLine("sourceFiles.Length: " + sourceFiles.Length);


      if (sourceFiles.Length > 0)
      {
        sourceObjectFiles = new MFilesAPI.SourceObjectFiles();

        /* add a source file */
        for (int i = 0; i < sourceFiles.Length; i++)
        {
          var sourceObjectFile = new MFilesAPI.SourceObjectFile();
          sourceObjectFile.SourceFilePath = sourceFiles[i].FilePath;
          sourceObjectFile.Title = sourceFiles[i].Title;
          sourceObjectFile.Extension = sourceFiles[i].Extension;
          sourceObjectFiles.Add(i, sourceObjectFile);
        }
      }

      return sourceObjectFiles;
    }

    public int Import(CsvModel csvModel)
    {
      var vault = this.VaultService.Vault.Value;

      if (this.ObjectTypeLookup == null)
        this.ObjectTypeLookup = GetObjTypeLookup(vault);
      var objectTypeLookup = this.ObjectTypeLookup;
      if (this.ObjectClassLookup == null)
        this.ObjectClassLookup = GetObjectClassLookup(vault);
      var objectClassLookup = this.ObjectClassLookup;
      if (this.PropertyDefLookupById == null)
        this.PropertyDefLookupById = GetPropertyDefLookupById(vault);
      var propertyDefLookupById = this.PropertyDefLookupById;
      if (this.PropertyDefLookupByName == null)
        this.PropertyDefLookupByName = GetPropertyDefLookupByName(vault);
      var propertyDefLookupByName = this.PropertyDefLookupByName;

      int objectClassId = objectClassLookup[csvModel.Class].ID;
      int objectTypeId = objectTypeLookup[csvModel.ObjectType].ID;
      Debug.WriteLine("objectTypeId: {0}, objectClassId: {1}", objectTypeId, objectClassId);

      ObjType objType = objectTypeLookup[csvModel.ObjectType];

      /* create the property Values */
      var propertyValues = GetPropertyValues(csvModel, vault, objectClassId, objType, objectTypeLookup,
        propertyDefLookupByName, propertyDefLookupById);

      /* attach the files */
      var sourceObjectFiles = GetSourceObjectFiles(csvModel);

      /* create the object */
      var objectVersionAndProperties = GetObjectVersionAndProperties(csvModel, vault, propertyValues, sourceObjectFiles, objectTypeId);

      //var objectVersionAndProperties = GetObjectVersionAndPropertiesAndCheckin(csvModel, vault, propertyValues, sourceObjectFiles, objectTypeId);

      /* check in the newly created object */
      vault.ObjectOperations.CheckIn(objectVersionAndProperties.ObjVer);

      /* return the id */
      return objectVersionAndProperties.ObjVer.ObjID.ID;
    }

    #endregion
  }
}
