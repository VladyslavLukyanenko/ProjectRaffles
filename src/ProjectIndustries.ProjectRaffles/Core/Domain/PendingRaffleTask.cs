using System;
using System.Collections.Generic;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class PendingRaffleTask : Entity
  {
    private static readonly TimeSpan LifeTime = TimeSpan.FromDays(7);

    public PendingRaffleTask()
    {
    }

    public PendingRaffleTask(RaffleTask task, Email email = null)
    {
      Email = email;
      CreatedAt = DateTimeOffset.Now;
      ExpiresAt = CreatedAt + LifeTime;
      ProxyGroup = task.ProxyGroupName;
      Proxy = task.Proxy;
      Profile = task.Profile;
      ProviderName = task.ProviderName;
      ProductName = task.ProductName;
      FieldValues = task.Module.AdditionalFields
        .Where(f => f is not HiddenField)
        .Select(f => new KeyValuePair<string, string>(f.DisplayName ?? f.SystemName, f.DisplayValue))
        .ToList();

      if (!email?.CanBeTracked() ?? true)
      {
        AssignRaffleStatus(task.Status.IsSuccessful);
      }
    }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }
    public DateTimeOffset? NotifiedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public string ProxyGroup { get; private set; }

    // public string SubmittedEmail { get; private set; }
    public Email Email { get; private set; }
    public Proxy Proxy { get; private set; }
    public Profile Profile { get; private set; }
    public string ProviderName { get; private set; }
    public string ProductName { get; private set; }
    public bool? IsWinner { get; private set; }
    public IList<KeyValuePair<string, string>> FieldValues { get; private set; }

    public uint? LastCheckedEmailId { get; set; }

    public bool IsConfirmed() => ConfirmedAt.HasValue;

    public void AssignRaffleStatus(bool isWinner)
    {
      if (IsWinner.HasValue)
      {
        throw new InvalidOperationException(
          $"Status of this raffle already assigned at {NotifiedAt}. Submitted email: " + Email.Value);
      }

      IsWinner = isWinner;
    }

    public bool IsExpired()
    {
      return ExpiresAt < DateTimeOffset.Now;
    }

    public void Notified()
    {
      if (NotifiedAt.HasValue)
      {
        throw new InvalidOperationException("Already notified at " + NotifiedAt);
      }

      NotifiedAt = DateTimeOffset.UtcNow;
    }

    public void Confirmed()
    {
      if (ConfirmedAt.HasValue)
      {
        throw new InvalidOperationException("Already confirmed at " + ConfirmedAt);
      }

      ConfirmedAt = DateTimeOffset.UtcNow;
    }

    public static PendingRaffleTask Create(RaffleTask task)
    {
      Email receiver = null;
      if (task.Module is IRaffleResultsEmailReceiver emailReceiver && emailReceiver.ReceiverEmail.CanBeTracked())
      {
        receiver = emailReceiver.ReceiverEmail;
      }

      return new PendingRaffleTask(task, receiver);
    }

    public bool CanBeTracked()
    {
      return Email?.CanBeTracked() ?? false;
    }
  }
}