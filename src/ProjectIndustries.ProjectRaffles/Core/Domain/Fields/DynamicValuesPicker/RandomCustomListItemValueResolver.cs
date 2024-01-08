using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.CustomLists;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class RandomCustomListItemValueResolver : RandomValueResolverBase
  {
    private readonly Random _random = new Random((int) DateTime.Now.Ticks);
    private IList<string> _list;
    private bool _requiresUniqueValues;

    public RandomCustomListItemValueResolver()
      : base("Random List Item")
    {
    }

    public override async Task PostProcessAsync(IReadonlyDependencyResolver dependencyResolver)
    {
      var customListRepository = dependencyResolver.GetService<ICustomListRepository>();
      var lists = customListRepository.LocalItems.ToList();
      if (lists.Count == 0)
      {
        throw new OperationCanceledException("Not lists found to pick from.");
      }

      var presenter = dependencyResolver.GetService<IValueResolverConfigurationPresenter>();
      var listOptions = lists.Where(_ => _.Items.Count > 0)
        .Select(l => new KeyValuePair<string, object>(l.Name, l))
        .ToArray();
      if (listOptions.Length == 0)
      {
        throw new OperationCanceledException("No lists with items found to pick from.");
      }

      var settings = new SelectField<CustomList>(displayName: "Please select list", options: listOptions);

      var onlyUnique = new CheckboxField<bool>(true, displayName: "Limit to unique values");
      await presenter.ShowConfigurationWindowAsync("Configure Custom List Item Picker", settings, onlyUnique);
      _list = (settings.Value ?? lists[_random.Next(0, lists.Count)]).Items.ToList();
      
      _requiresUniqueValues = onlyUnique.IsChecked;
    }

    protected override string GetNonUniqueRandomValue()
    {
      return _list[_random.Next(0, _list.Count)];
    }

    protected override string GetUniqueRandomValue()
    {
      var item = GetNonUniqueRandomValue();
      _list.Remove(item);

      return item;
    }

    protected override bool CanGenerateUniqueRandomValue => _list.Count != 0;
    protected override bool RequiresUniqueValues => _requiresUniqueValues;
  }
}