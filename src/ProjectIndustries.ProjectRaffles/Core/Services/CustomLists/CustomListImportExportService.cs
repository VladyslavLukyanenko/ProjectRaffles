using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.CustomLists
{
  public class CustomListImportExportService : ImportExportServiceBase<CustomList>, ICustomListImportExportService
  {
    public CustomListImportExportService(ICustomListRepository repository)
      : base(repository)
    {
    }

    public override async Task ExportAsCsvAsync(Stream output, CancellationToken ct)
    {
      var lists = await Repository.GetAllAsync(ct);
      using (var writer = new StreamWriter(output))
      using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
      {
        foreach (var customList in lists)
        {
          csv.WriteField(customList.Name, true);
        }

        await csv.NextRecordAsync();

        var groupsCount = lists.Count;
        var rows = lists.Max(_ => _.Items.Count);
        for (var row = 0; row < rows; row++)
        {
          for (var col = 0; col < groupsCount; col++)
          {
            var list = lists[col];
            var item = list.Items.Count > row ? list.Items[row] : "";
            csv.WriteField(item, true);
          }

          await csv.NextRecordAsync();
        }
      }
    }

    public override async Task<bool> ImportFromCsvAsync(Stream input, CancellationToken ct)
    {
      List<string[]> rawGroups = new List<string[]>();

      using (var reader = new StreamReader(input))
      using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
      {
        string[] row;
        while ((row = await csv.Parser.ReadAsync()) != null)
        {
          rawGroups.Add(row);
        }
      }

      if (rawGroups.Count == 0)
      {
        return false;
      }

      var groups = rawGroups[0]
        .Select(name => new CustomList(name, Array.Empty<string>()))
        .ToArray();
      for (var row = 1; row < rawGroups.Count; row++)
      {
        var items = rawGroups[row];
        for (var col = 0; col < items.Length; col++)
        {
          var group = groups[col];
          var item = items[col];
          if (string.IsNullOrEmpty(item))
          {
            continue;
          }

          @group.Items.Add(item);
        }
      }

      await Repository.SaveAsync(groups, ct);
      return true;
    }
  }
}