using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Test
{
  public static class TestData
  {
    public static string GetTestAssemblyFolder
    {
      get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
    }

    public static string TestDataFolder
    {
      get { return Path.Combine(GetTestAssemblyFolder, @"..\..\TestData"); }
    }

    public static string GetTestFile(string fileName)
    {
      return Path.Combine(TestDataFolder, fileName);
    }
  }
}
