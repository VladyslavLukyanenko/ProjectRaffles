using System.Collections.Generic;
using DynamicData.Binding;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class CustomList : Entity
  {
    private CustomList()
    {
    }

    public CustomList(string name, IEnumerable<string> items)
    {
      Name = name;
      Items.AddRange(items);
    }

    public string Name { get; private set; }
    public ObservableCollectionExtended<string> Items { get; set; } = new ObservableCollectionExtended<string>();
  }
}