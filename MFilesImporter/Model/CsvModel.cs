using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Model
{
  class CsvModel
  {
    public string ObjectType { get; set; }
    public string Class { get; set; }
    public List<string> SourceFiles { get; private set; }
    public Dictionary<string, string> Values { get; private set; }

    public CsvModel()
    {
      this.SourceFiles = new List<string>();
      this.Class = this.ObjectType = string.Empty;
      this.Values = new Dictionary<string, string>();
    }

    public override string ToString()
    {
      StringBuilder strBuilder = new StringBuilder();
      strBuilder.AppendLine(string.Format("ObjectType={0}", this.ObjectType));
      strBuilder.AppendLine(string.Format("Class={0}", this.Class));
      foreach (var kv in this.Values)
        strBuilder.AppendFormat("{0}{1}: {2}", Environment.NewLine, kv.Key, kv.Value);
      strBuilder.AppendLine();
      return strBuilder.ToString();
    }
  }
}
