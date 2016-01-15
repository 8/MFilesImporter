using MFilesImporter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFilesImporter.Service
{
  interface ICsvWriterService
  {
    void Write(IEnumerable<CsvModel> models, TextWriter writer, string delimiter  = ",");
  }

  static class CsvWriterServiceExtensions
  {
    public static void Write(this ICsvWriterService csvWriterService, IEnumerable<CsvModel> models, string file, string delimiter = ",")
    {
      using (var fs = File.Create(file))
      using (var tw = new StreamWriter(fs, Encoding.Default))
        csvWriterService.Write(models, tw, delimiter);
    }
  }

  class CsvWriterService : ICsvWriterService
  {
    public CsvWriterService()
    {
    }

    public void Write(IEnumerable<CsvModel> models, TextWriter writer, string delimiter)
    {
      /* get all headers row */
      List<string> headers = new List<string>();
      headers.Add(CsvHeaders.ObjectType);
      headers.Add(CsvHeaders.Class);
      headers.Add(CsvHeaders.Source);

      foreach (var model in models)
      {
        foreach (string key in model.Values.Keys)
          if (!headers.Contains(key))
            headers.Add(key);
      }

      using (var csvWriter = new CsvHelper.CsvWriter(writer,
        new CsvHelper.Configuration.CsvConfiguration() {
          QuoteAllFields = true,
          Delimiter = delimiter }
        ))
      {
        /* write the header row */
        foreach (string header in headers)
          csvWriter.WriteField<string>(header);
        csvWriter.NextRecord();

        /* write the content of each csv model */
        foreach (var model in models)
        {
          for (int i = 0; i < headers.Count; i++)
          {
            string value = string.Empty;
            switch (i)
            {
              case 0: value = model.ObjectType; break;
              case 1: value = model.Class; break;
              case 2: value = string.Join(new string (new char[] {Constants.SourceFileDelimiter }), model.SourceFiles); break;
              default:
                if (!model.Values.TryGetValue(headers[i], out value))
                  value = string.Empty;
                break;
            }
            csvWriter.WriteField<string>(value);
          }
          csvWriter.NextRecord();
        }
      }
    }
  }
}
