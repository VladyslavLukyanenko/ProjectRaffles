using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Validators
{
  public class DiscordSettingsValidator : AbstractValidator<DiscordSettings>
  {
    const string NotAvailableMessage = "Provided webhook url is not available or invalid. Please check it again.";
    const string InvalidUrlMessage = "Invalid discord webhook url provided.";

    public DiscordSettingsValidator()
    {
      RuleFor(_ => _.WebHook).Must(BeValidUrl).WithMessage(InvalidUrlMessage)
        .MustAsync(BeValidHook).WithMessage(NotAvailableMessage);
    }

    private bool BeValidUrl(DiscordSettings settings, string url, PropertyValidatorContext ctx)
    {
      return !string.IsNullOrEmpty(url) && url.Contains("discord", StringComparison.InvariantCultureIgnoreCase);
    }

    private async Task<bool> BeValidHook(DiscordSettings settings, string url, PropertyValidatorContext ctx,
      CancellationToken ct)
    {
      if (string.IsNullOrEmpty(url) || !BeValidUrl(settings, url, ctx))
      {
        return false;
      }

      var client = new HttpClient();
      var resp = await client.GetAsync(url, ct);
      if (!resp.IsSuccessStatusCode)
      {
        return false;
      }

      var json = await resp.Content.ReadAsStringAsync(ct);
      var obj = JObject.Parse(json);

      return obj.ContainsKey("name");
    }
  }
}