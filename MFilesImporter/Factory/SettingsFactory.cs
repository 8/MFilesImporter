using MFilesImporter.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Factory
{
  interface ISettingsFactory
  {
    SettingsModel GetSettings();
  }

  class SettingsFactory : ISettingsFactory
  {
    public SettingsModel GetSettings()
    {
      return new SettingsModel()
      {
        User = ConfigurationManager.AppSettings["user"],
        Password = ConfigurationManager.AppSettings["password"],
        Vault = ConfigurationManager.AppSettings["vault"],
        Delimiter = ConfigurationManager.AppSettings["delimiter"],
        Server = ConfigurationManager.AppSettings["server"]
      };
    }
  }
}
