using MFilesImporter.Factory;
using MFilesImporter.Model;
using MFilesImporter.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface IParametersService
  {
    ParametersModel Parameters { get; }
  }

  class ParametersService : IParametersService
  {
    public ParametersModel Parameters { get; private set; }

    public ParametersService(IParametersFactory parametersFactory,
                             ArgumentsModel arguments)
    {
      this.Parameters = parametersFactory.Parse(arguments.Arguments);
    }
  }
}
