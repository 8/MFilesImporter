using MFilesImporter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter
{
  class EntryPoint
  {
    static int Main(string[] args)
    {
      TinyIoC.TinyIoCContainer container = new TinyIoC.TinyIoCContainer();
      container.Register<ArgumentsModel>(new ArgumentsModel(args));
      container.RegisterInterfaceImplementations("MFilesImporter.Service", TinyIoCExtensions.RegisterOptions.AsSingleton, TinyIoCExtensions.RegisterTypes.AsInterfaceTypes);
      container.RegisterInterfaceImplementations("MFilesImporter.Factory", TinyIoCExtensions.RegisterOptions.AsSingleton, TinyIoCExtensions.RegisterTypes.AsInterfaceTypes);

      var program = container.Resolve<Program>();
      return program.Run();
    }
  }
}
