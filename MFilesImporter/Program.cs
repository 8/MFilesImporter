using MFilesImporter.Factory;
using MFilesImporter.Model;
using MFilesImporter.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter
{
  class Program
  {
    private readonly IParametersService ParametersService;
    private readonly IConnectionTestService ConnectionTestService;
    private readonly ICsvImporterService CsvImporterService;
    private readonly ICsvParserService CsvParserService;
    private readonly IResultWriterService ResultWriterService;
    private readonly ICsvWriterService CsvWriterService;
    private readonly IDestroyObjectService DestroyObjectService;
    private readonly IFindObjectsService FindObjectsService;
    private readonly IClassesService ClassesService;
    private readonly IObjectTypeService ObjectTypeService;

    public Program(IParametersService parametersService,
                   IConnectionTestService connectionTestService,
                   ICsvImporterService csvImporterService,
                   ICsvParserService csvParserService,
                   IResultWriterService resultWriterService,
                   ICsvWriterService csvWriterService,
                   IDestroyObjectService destroyObjectService,
                   IFindObjectsService findObjectsService,
                   IClassesService classesService,
                   IObjectTypeService objectTypeService)
    {
      this.ParametersService = parametersService;
      this.ConnectionTestService = connectionTestService;
      this.CsvImporterService = csvImporterService;
      this.CsvParserService = csvParserService;
      this.ResultWriterService = resultWriterService;
      this.CsvWriterService = csvWriterService;
      this.DestroyObjectService = destroyObjectService;
      this.ClassesService = classesService;
      this.FindObjectsService = findObjectsService;
      this.ObjectTypeService = objectTypeService;
    }

    #region Methods

    private const string ImportStatusSuccess = "Success";

    internal int Run()
    {
      var p = this.ParametersService.Parameters;

      int ret;
      try { ret = Run(p); }
      catch (Exception ex)
      {
        Log(ex.ToString());
        ret = 1;
      }
      return ret;
    }

    internal static string ExtendFileName(string filePath, string addition)
    {
      string baseFilePath = Path.GetFileNameWithoutExtension(filePath);
      string extension = Path.GetExtension(filePath);
      return baseFilePath + addition + extension;
    }

    internal static string GetOutputFilePath(string inputFilePath, DateTime dt)
    {
      string addition = string.Format("_result_{0}", dt.ToString("yyyy-MM-dd_HHmmss"));
      return ExtendFileName(inputFilePath, addition);
    }

    internal static string GetBackupFilePath(string inputFilePath, DateTime dt)
    {
      string addition = string.Format("_imported_{0}", dt.ToString("yyyy-MM-dd_HHmmss"));
      return ExtendFileName(inputFilePath, addition);
    }

    private void CsvImport(ParametersModel p)
    {
      /* listen for CTRL-C */
      bool cancelRequested = false;
      Console.CancelKeyPress += (o, e) => { e.Cancel = true; cancelRequested = true; };

      Stopwatch sw = Stopwatch.StartNew();

      var successful = new List<CsvModel>();
      var unsuccessful = new List<CsvModel>();
      var ignored = new List<CsvModel>();

      string delimiter = p.Delimiter != string.Empty ? p.Delimiter : ",";

      DateTime now = DateTime.Now;
      string outputFile = GetOutputFilePath(p.CsvImportFile, now);

      Log(string.Format("reading Csv file: '{0}'", p.CsvImportFile));
      var csvModels = this.CsvParserService.GetCsvModels(p.CsvImportFile, delimiter).ToArray();
      Log(string.Format("found {0} models", csvModels.Length));

      /* lock the input file for exclusive access, as we want to move it later on */
      using (var fsInput = File.OpenWrite(p.CsvImportFile))
      {
        Log(string.Format("writing result to file: {0}", outputFile));
        using (var fsOutput = File.OpenWrite(outputFile))
        using (var twOutput = new StreamWriter(fsOutput, Encoding.Default))
        {
          Log(string.Format("logging successful imports to file: '{0}'", p.SuccessFile));
          Log(string.Format("logging unsuccessful imports to file: '{0}'", p.FailureFile));
          int index = 0;
          int tried = 0;
          using (var successWriter = this.ResultWriterService.GetSuccessWriter())
          using (var failureWriter = this.ResultWriterService.GetFailureWriter())
          {
            foreach (var csvModel in csvModels)
            {
              /* ignore already successful imports */
              string importStatus = null;
              if (csvModel.Values.TryGetValue(CsvHeaders.ImportStatus, out importStatus) && importStatus == ImportStatusSuccess)
                ignored.Add(csvModel);
              else
              {
                try
                {
                  tried++;
                  int id = this.CsvImporterService.Import(csvModel);
                  successful.Add(csvModel);
                  successWriter.WriteLine(csvModel.ToString());
                  csvModel.Values[CsvHeaders.ImportStatus] = ImportStatusSuccess;
                  csvModel.Values[CsvHeaders.Id] = id.ToString();
                }
                catch (Exception ex)
                {
                  unsuccessful.Add(csvModel);
                  csvModel.Values[CsvHeaders.ImportStatus] = "failed due to error: " + ex.Message;
                  failureWriter.WriteLine(string.Format("failed to import:"));
                  failureWriter.WriteLine(csvModel.ToString());
                  failureWriter.WriteLine(string.Format("because of the following error:"));
                  failureWriter.WriteLine(ex);
                }
              }

              int successRate = (int)(successful.Count * 100f / (tried != 0 ? tried : 1));
              int failureRate = (int)(unsuccessful.Count * 100f / (tried != 0 ? tried : 1));
              int ignoredRate = (int)(ignored.Count * 100f / (index != 0 ? index :  1));
              int percent = (int)(++index * 100f / csvModels.Length);

              Log(string.Format("Progress: {0}/{1} ({2} %)            ", index, csvModels.Length, percent));
              Log(string.Format("Success:  {0}/{1} ({2} %)            ", successful.Count, tried, successRate));
              Log(string.Format("Failure:  {0}/{1} ({2} %)            ", unsuccessful.Count, tried, failureRate));
              Log(string.Format("Ignored:  {0}/{1} ({2} %)            ", ignored.Count, csvModels.Length, ignoredRate));

              if (cancelRequested)
              {
                Log("Import cancelled!");
                break;
              }

              if (index < csvModels.Length)
                Console.CursorTop = Console.CursorTop - 4;
            }

            this.CsvWriterService.Write(csvModels, twOutput, delimiter);
          }

        }
        Log(string.Format("ignored {0}/{1} ({2:0.00} %) as they were already successfully imported",
          ignored.Count, csvModels.Length, ignored.Count * 100f / csvModels.Length));

        Log(string.Format("imported {0}/{1} ({2:0.00} %) successfully",
          successful.Count, csvModels.Length - ignored.Count, successful.Count * 100f / (csvModels.Length - ignored.Count)));
        sw.Stop();
        Log(string.Format("importing took {0}", sw.Elapsed));
      }
      /* rename the input file */
      string backupFile = GetBackupFilePath(p.CsvImportFile, now);
      Log(string.Format("backing up input file to '{0}'", backupFile));
      File.Move(p.CsvImportFile, backupFile);

      /* rename the output file */
      Log(string.Format("replacing the input file"));
      File.Move(outputFile, p.CsvImportFile);
    }

    private void DestroyObjectsByClass(ParametersModel p)
    {
      foreach (string className in p.DestroyObjectOfClasses)
      {
        Log(string.Format("destroying objects of class: '{0}'", className));
        int count = this.DestroyObjectService.DestroyObjectsByClass(className);
        Log(string.Format("destroyed {0} objects", count));
      }
    }

    private void DestroyObjectsByObjectType(ParametersModel p)
    {
      foreach (string objectType in p.DestroyObjectsOfObjectTypes)
      {
        Log(string.Format("destroying objects of object type: '{0}'", objectType));
        int count = this.DestroyObjectService.DestroyObjectsByObjectType(objectType);
        Log(string.Format("destroyed {0} objects", count));
      }
    }

    private void ListClassesAndObjects(ParametersModel p)
    {
      var classes = this.ClassesService.GetClasses();

      foreach (string className in classes)
      {
        var searchResults = this.FindObjectsService.FindObjectsByClass(className);
        Log(string.Format("class '{0}' has {1} objects", className, searchResults.Count));
      }
    }

    private void ListObjectTypesAndObjects(ParametersModel p)
    {
      var objectTypes = this.ObjectTypeService.GetObjectTypes();

      foreach (string objectType in objectTypes)
      {
        var searchResults = this.FindObjectsService.FindObjectsByObjectType(objectType);
        Log(string.Format("object type: '{0}' has {1} objects", objectType, searchResults.Count));
      }
    }

    private void WriteDescription(ParametersModel p)
    {
      var writer = GetLogWriter();
      writer.WriteLine("MFilesImporter written by martin kramer <martin.kramer@lostindetails.com>");
      p.Options.WriteOptionDescriptions(writer);
    }

    private int Run(ParametersModel p)
    {
      if ((p.Action & ActionType.TestConnection) == ActionType.TestConnection)
        Log(this.ConnectionTestService.TestConnection() ? "Connection successful" : "Connection failed.");

      if ((p.Action & ActionType.WriteDescripton) == ActionType.WriteDescripton)
        WriteDescription(p);

      if ((p.Action & ActionType.CsvImport) == ActionType.CsvImport)
        CsvImport(p);

      if ((p.Action & ActionType.DestroyObjectsOfClasses) == ActionType.DestroyObjectsOfClasses)
        DestroyObjectsByClass(p);

      if ((p.Action & ActionType.DestroyObjectsOfObjectTypes) == ActionType.DestroyObjectsOfObjectTypes)
        DestroyObjectsByObjectType(p);

      if ((p.Action & ActionType.ListClassesAndObjects) == ActionType.ListClassesAndObjects)
        ListClassesAndObjects(p);

      if ((p.Action & ActionType.ListObjectTypesAndObjects) == ActionType.ListObjectTypesAndObjects)
        ListObjectTypesAndObjects(p);

      return 0;
    }

    private void Log(string text)
    {
      GetLogWriter().WriteLine(text);
    }

    private TextWriter GetLogWriter()
    {
      return Console.Out;
    }

    #endregion
  }
}
