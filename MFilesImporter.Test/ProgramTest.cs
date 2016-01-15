using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Test
{
  [TestClass]
  public class ProgramTest
  {
    [TestMethod]
    public void ProgramTest_GetOutputFilePath()
    {
      string outputFile = Program.GetOutputFilePath("test.csv", new DateTime(2015, 06, 25, 11, 10, 36));
      Assert.AreEqual<string>("test_result_2015-06-25_111036.csv", outputFile);
    }
  }
}
