using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Model
{
  class SettingsModel
  {
    public string User { get; set; }
    public string Password { get; set; }
    public string Vault { get; set; }
    public string Server { get; set; }
    public string BaseFolder { get; set; }
    public string Delimiter { get; set; }

    public SettingsModel()
    {
      this.User = this.Password = this.Vault = this.Delimiter = this.Server = string.Empty;
    }
  }
}
