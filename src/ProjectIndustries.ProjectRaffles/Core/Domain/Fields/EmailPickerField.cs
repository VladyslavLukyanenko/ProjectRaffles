using System;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields
{
  public class EmailPickerField : Field<Email>
  {
    private Email _materializedValue;

    public EmailPickerField()
    {
      ResetMaterializedValueOnChanged();
    }

    public EmailPickerField(string systemName = null, string displayName = null, bool isRequired = true)
      : base(systemName, displayName, isRequired)
    {
      ResetMaterializedValueOnChanged();
    }

    private void ResetMaterializedValueOnChanged()
    {
      Changed.Subscribe(_ => { _materializedValue = null; });
    }

    public override string ValueId => MaterializedValue?.Value;
    public override string DisplayValue => ValueId;

    public Email MaterializedValue => _materializedValue ??= Value?.MaterializeEmail();
  }
}