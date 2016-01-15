using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Model
{
  class ArgumentsModel
  {
    public string[] Arguments { get; private set; }

    public ArgumentsModel(string[] arguments)
    {
      this.Arguments = arguments;
    }
  }
}
