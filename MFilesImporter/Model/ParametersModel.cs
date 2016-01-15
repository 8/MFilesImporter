using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Model
{
  [Flags]
  enum ActionType
  {
    WriteDescripton = 1,
    TestConnection = 2,
    ParseOnly = 4,
    CsvImport = 32,
    DestroyObjectsOfClasses = 64,
    ListClassesAndObjects = 128,
    ListObjectTypesAndObjects = 256,
    DestroyObjectsOfObjectTypes = 512,
  }

  class ParametersModel
  {
    public string BaseFolder { get; set; }
    public Mono.Options.OptionSet Options { get; set; }
    public ActionType Action { get; set; }

    public string Vault { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string Server { get; set; }
    public string CsvImportFile { get; set; }
    public string SuccessFile { get; set; }
    public string FailureFile { get; set; }
    public string Delimiter { get; set; }
    public string[] DestroyObjectOfClasses { get; set; }
    public string[] DestroyObjectsOfObjectTypes { get; set; }

    public ParametersModel()
    {
      this.BaseFolder = this.Vault = this.CsvImportFile = this.Delimiter = this.Server = string.Empty;
      this.Action = ActionType.WriteDescripton;
      this.SuccessFile = "import_success.txt";
      this.FailureFile = "import_failure.txt";
      this.DestroyObjectOfClasses = new string[0];
      this.DestroyObjectsOfObjectTypes = new string[0];
    }
  }
}
