using MFilesImporter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Factory
{
  interface IParametersFactory
  {
    ParametersModel Parse(string[] args);
  }

  class ParametersFactory : IParametersFactory
  {
    public ParametersModel Parse(string[] args)
    {
      var p = new ParametersModel();
      var options = new Mono.Options.OptionSet();

      options.Add("h|help", "prints the help", s => p.Action = ActionType.WriteDescripton);
      options.Add("t|test-connection", "tries to connect to the server using the specified credentials", s => p.Action = ActionType.TestConnection);
      options.Add("b|base-folder=", "use the supplied directory as the base folder for the import", s => p.BaseFolder = s);
      options.Add("v|vault=", "use the specified vault", s => p.Vault = s);
      options.Add("u|user=", "use the specified user", s => p.User = s);
      options.Add("s|server=", "use the specified server", s => p.Server = s);
      options.Add("p|password=", "use the specified password", s => p.Password = s);
      options.Add("c|csv=", "import based on the csv file", s => { p.Action = ActionType.CsvImport; p.CsvImportFile = s; });
      options.Add("d|delimiter=", "csv delimiter", s => p.Delimiter = s);
      options.Add("destroy-by-class=", "destroy objects of the following classes, use Comma ',' as a separator for multiple classes",
        s => { 
          p.Action = ActionType.DestroyObjectsOfClasses;
          p.DestroyObjectOfClasses = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        });
      options.Add("destroy-by-objecttype=", "destroy objects of the specified object types, use Comma ',' as a separator for multiple classes",
        s =>
        {
          p.Action = ActionType.DestroyObjectsOfObjectTypes;
          p.DestroyObjectsOfObjectTypes = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        });
      options.Add("lc|list-classes", "list all classes and their object counts", s => p.Action = ActionType.ListClassesAndObjects);
      options.Add("lo|list-objecttypes", "list all object types and their object counts", s => p.Action = ActionType.ListObjectTypesAndObjects);

      options.Parse(args);
      p.Options = options;
      return p;
    }
  }
}
