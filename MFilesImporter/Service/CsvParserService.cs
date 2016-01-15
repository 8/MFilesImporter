using CsvHelper;
using MFilesImporter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface ICsvParserService
  {
    IEnumerable<CsvModel> GetCsvModels(TextReader tr, string delimiter = ",");
  }

  static class CsvParserServiceExtensions
  {
    public static IEnumerable<CsvModel> GetCsvModels(this ICsvParserService csvParserService, string csvFile)
    {
      using (var fs = File.OpenRead(csvFile))
      using (var tr = new StreamReader(fs, Encoding.Default))
      {
        return csvParserService.GetCsvModels(tr);
      }
    }

    public static IEnumerable<CsvModel> GetCsvModels(this ICsvParserService csvParserService, string csvFile, string delimiter)
    {
      using (var fs = File.OpenRead(csvFile))
      using (var tr = new StreamReader(fs, Encoding.Default))
      {
        return csvParserService.GetCsvModels(tr, delimiter);
      }
    }
  }

  static class StringArrayExtensions
  {
    public static int GetIndexOf(this string[] stringArray, string value)
    {
      for (int i = 0; i < stringArray.Length; i++)
        if (stringArray[i].ToLower() == value.ToLower())
          return i;
      return -1;
    }
  }

  static class CsvHeaders
  {
    public const string Class = "Class";
    public const string ObjectType = "Object Type";
    public const string Source = "Source";
    public const string ImportStatus = "Import Status";
    public const string Id = "Id";
  }

  static class Constants
  {
    public const char SourceFileDelimiter = '|';
    public const char MultiSelectDelimiter = '|';
  }

  class CsvParserService : ICsvParserService
  {
    public IEnumerable<CsvModel> GetCsvModels(TextReader tr, string delimiter)
    {
      List<CsvModel> models = new List<CsvModel>();
      using (var csvParser = new CsvParser(tr,
        new CsvHelper.Configuration.CsvConfiguration() { Delimiter = delimiter }
        ))
      {
        var headerRow = csvParser.Read();

        int classIndex = headerRow.GetIndexOf(CsvHeaders.Class);
        int objectTypeIndex = headerRow.GetIndexOf(CsvHeaders.ObjectType);
        int sourceIndex = headerRow.GetIndexOf(CsvHeaders.Source);

        if (classIndex == -1)
          throw new ArgumentException("missing column 'Class'!");

        if (objectTypeIndex == -1)
          throw new ArgumentException("missing column 'Object Type'!");

        string[] row = null;
        while ((row = csvParser.Read()) != null)
        {
          var csvModel = new CsvModel();
          csvModel.ObjectType = row[objectTypeIndex];
          csvModel.Class = row[classIndex];

          for (int i = 0; i < row.Length; i++)
          {
            if (i == sourceIndex)
            {
              var sourceFiles = row[i].Split(new char[] { Constants.SourceFileDelimiter }, StringSplitOptions.RemoveEmptyEntries);
              foreach (var sourceFile in sourceFiles)
                csvModel.SourceFiles.Add(sourceFile);
            }
            else if (i != classIndex &&
                i != objectTypeIndex &&
                headerRow[i] != CsvHeaders.Source)
              csvModel.Values.Add(headerRow[i], row[i]);
          }

          models.Add(csvModel);
        }
      }
      return models;
    }
  }
}
