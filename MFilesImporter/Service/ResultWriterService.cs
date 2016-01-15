using MFilesImporter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface IResultWriterService
  {
    TextWriter GetSuccessWriter();
    TextWriter GetFailureWriter();
  }

  class ResultWriterService : IResultWriterService
  {
    private readonly IParametersService ParametersService;

    public ResultWriterService(IParametersService parametersService)
    {
      this.ParametersService = parametersService;
    }

    private TextWriter GetWriter(string filePath)
    {
      var fs = File.Create(filePath);
      return new StreamWriter(fs, Encoding.Default);
    }

    public TextWriter GetSuccessWriter()
    {
      return GetWriter(this.ParametersService.Parameters.SuccessFile);
    }

    public TextWriter GetFailureWriter()
    {
      return GetWriter(this.ParametersService.Parameters.FailureFile);
    }
  }
}
