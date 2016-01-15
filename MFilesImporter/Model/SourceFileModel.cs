using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Model
{
  enum DocumentType
  {
    Unknown,
    Reconciliation,
    RSI,
    Vertrag
  }

  class SourceFileModel
  {
    public string FilePath { get; set; }
    public string Extension { get; set; }
    public string Title { get; set; }

    public SourceFileModel()
    {
    }

    internal static string GetTitle(string file)
    {
      return Path.GetFileNameWithoutExtension(file);
    }

    public static SourceFileModel FromFile(string file)
    {
      var sourceFile = new SourceFileModel()
      {
        Title = GetTitle(file),
        Extension = Path.GetExtension(file).TrimStart('.'),
        FilePath = file
      };
      return sourceFile;
    }

    public override string ToString()
    {
      return string.Format("FilePath={0}, Extension={1}, Title={2}",
        this.FilePath, this.Extension, this.Title);
    }

  }

  //class DocumentModel
  //{
  //  public DateTime Datum { get; set; }
  //  public DocumentType Type { get; set; }
  //  public List<SourceFileModel> SourceFiles { get; private set; }
  //  public string Kunde { get; set; }

  //  public DocumentModel()
  //  {
  //    this.SourceFiles = new List<SourceFileModel>();
  //  }

  //  public override string ToString()
  //  {
  //    var strBuilder = new StringBuilder(
  //      string.Format("Type={0}, Datum={1}, Kunde={2}, SourceFiles.Count={3}, SourceFiles=",
  //      this.Type, this.Datum, this.Kunde, this.SourceFiles.Count));

  //    foreach (var sourceFile in this.SourceFiles)
  //      strBuilder.Append(sourceFile.ToString());
  //    return strBuilder.ToString();
  //  }
  //}
}
