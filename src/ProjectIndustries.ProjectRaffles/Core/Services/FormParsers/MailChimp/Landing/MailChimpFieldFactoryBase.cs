using System.Collections.Generic;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Landing
{
  public abstract class MailChimpFieldFactoryBase<T> : IMailChimpFieldFactory
    where T: MailChildFieldBase, new()
  {
    private readonly IEnumerable<string> _supportedFieldTypes;

    protected MailChimpFieldFactoryBase(params string[] supportedFieldTypes)
    {
      _supportedFieldTypes = supportedFieldTypes;
    }

    public bool IsSupports(string type) => _supportedFieldTypes.Contains(type);

    public MailChildFieldBase Create(SettingsField field)
    {
      var mailChimpField = CreateMailChimpField<T>(field);
      SetSpecificFieldProps(field, mailChimpField);

      return mailChimpField;
    }

    protected virtual void SetSpecificFieldProps(SettingsField source, T destination)
    {
    }

    private static TMailChimpField CreateMailChimpField<TMailChimpField>(SettingsField source)
      where TMailChimpField : MailChildFieldBase, new()
    {
      return new TMailChimpField
      {
        Label = source.Label,
        Name = source.Name,
        Type = source.Type,
        IsRequired = source.IsRequired,
        MergeId = source.MergeId
      };
    }
  }
}