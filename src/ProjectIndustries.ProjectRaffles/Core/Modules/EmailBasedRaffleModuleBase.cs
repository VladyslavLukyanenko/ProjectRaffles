using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public abstract class EmailBasedRaffleModuleBase<T> : RegularRaffleModuleBase<T>
    where T : IModuleHttpClient
  {
    protected readonly Field<string> EmailField = new DynamicValuesPickerField("Email", Pickers.All)
    {
      SelectedResolver = Pickers.Misc.Email
    };

    protected EmailBasedRaffleModuleBase(T client, string raffleUrlRegexPattern)
      : base(client, raffleUrlRegexPattern)
    {
    }

    protected override IEnumerable<object> GetDeclaredFields()
    {
      yield return EmailField;
    }
  }
}