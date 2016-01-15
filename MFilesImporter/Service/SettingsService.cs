using MFilesImporter.Factory;
using MFilesImporter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface ISettingsService
  {
    SettingsModel Settings { get; }
  }

  class SettingsService : ISettingsService
  {
    public SettingsModel Settings { get; private set; }

    public SettingsService(ISettingsFactory settingsFactory)
    {
      this.Settings = settingsFactory.GetSettings();
    }
  }
}
