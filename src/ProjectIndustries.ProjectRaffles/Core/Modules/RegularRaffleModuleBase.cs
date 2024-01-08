using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public abstract class RegularRaffleModuleBase<T> : RaffleModuleBase
    where T : IModuleHttpClient
  {
    protected readonly Field<string> RaffleUrl;

    protected RegularRaffleModuleBase(T client, string raffleUrlRegexPattern, bool skipFieldsInitialization = false)
      : base(true)
    {
      HttpClient = client;
      RaffleUrl = new TextField(displayName: "Raffle Url", validator: FieldValidators.IsMatches(raffleUrlRegexPattern));

      if (!skipFieldsInitialization)
      {
        _ = InitializeDefaultIdentityFields();
      }
    }

    protected override IModuleHttpClient HttpClient { get; }
    protected T Client => (T) HttpClient;

    public override IEnumerable<Field> AdditionalFields
    {
      get
      {
        yield return RaffleUrl;

        foreach (var declaredField in GetDeclaredFields())
        {
          if (declaredField is Field f)
          {
            yield return f;
          }
          else if (declaredField is IEnumerable flds)
          {
            foreach (var field in flds.Cast<Field>().Where(fld => fld != null))
            {
              yield return field;
            }
          }
          else
          {
            throw new InvalidOperationException(
              "Invalid type of declared field value. It can be either 'Field' or 'IEnumerable<Field>' but got "
              + declaredField.GetType().Name);
          }
        }
      }
    }

    protected override async Task ExecuteAsync(Profile profile, CancellationToken ct)
    {
      Status = await ExecuteAsync(ct) ? RaffleStatus.Succeeded : RaffleStatus.Failed;
    }

    protected abstract Task<bool> ExecuteAsync(CancellationToken ct);

    protected abstract IEnumerable<object> GetDeclaredFields();
  }
}