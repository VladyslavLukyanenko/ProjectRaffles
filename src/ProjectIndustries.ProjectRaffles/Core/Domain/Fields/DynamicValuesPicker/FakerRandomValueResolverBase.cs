using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public abstract class FakerRandomValueResolverBase : RandomValueResolverBase
  {
    private readonly string _configWindowTitle;

    private static readonly Dictionary<string, string> NonStandardLocales = new Dictionary<string, string>
    {
      {"af_ZA", "Afrikaans"},
      {"cz", "Czech"},
      {"en_au_ocker", "English (Australia Ocker)"},
      {"en_BORK", "English (Bork)"},
      {"en_IND", "English (India)"},
      {"ge", "Georgian"},
      {"nep", "Nepalese"},
    };

    private static readonly KeyValuePair<string, object>[] Locales = Database.GetAllLocales()
      .Select(locale =>
      {
        string cultureName;
        if (!NonStandardLocales.TryGetValue(locale, out cultureName))
        {
          try
          {
            var c = CultureInfo.GetCultureInfo(locale);
            cultureName = c.DisplayName;
          }
          catch
          {
            return null;
          }
        }

        return new
        {
          Locale = locale,
          CultureName = cultureName
        };
      })
      .Where(c => c != null)
      .Select(c => new KeyValuePair<string, object>(c.CultureName ?? c.Locale, c.Locale))
      .ToArray();

    private static readonly string FallbackLocale = "en";

    protected Faker Faker;

    protected FakerRandomValueResolverBase(string name, string configWindowTitle)
      : base(name)
    {
      _configWindowTitle = configWindowTitle;
    }

    public override async Task PostProcessAsync(IReadonlyDependencyResolver dependencyResolver)
    {
      var presenter = dependencyResolver.GetService<IValueResolverConfigurationPresenter>();
      var localeSelect = new SelectField<string>(displayName: "Target Locale", options: Locales)
      {
        Value = FallbackLocale
      };

      var configFields = GetConfigFields().ToList();
      configFields.Add(localeSelect);
      // var onlyUnique = new CheckboxField<bool>(true, displayName: "Limit to unique values");
      await presenter.ShowConfigurationWindowAsync(_configWindowTitle, configFields.ToArray());

      Faker = new Faker(localeSelect.Value);
    }


    protected override bool CanGenerateUniqueRandomValue => true;

    protected virtual IEnumerable<Field> GetConfigFields()
    {
      yield break;
    }
  }
}