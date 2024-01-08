using System;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Modules
{
  public class RaffleStatus : IEquatable<RaffleStatus>
  {
    public static RaffleStatus Preparation { get; } =
      new RaffleStatus(nameof(Preparation), RaffleStatusKind.Preparation);

    public static RaffleStatus Ready { get; } =
      new RaffleStatus(nameof(Ready), RaffleStatusKind.Ready, "Ready to start");

    public static RaffleStatus Failed { get; } = new RaffleStatus(nameof(Failed), RaffleStatusKind.Failed);
    public static RaffleStatus Succeeded { get; } = new RaffleStatus(nameof(Succeeded), RaffleStatusKind.Succeeded);

    public static RaffleStatus Cancelled { get; } =
      new RaffleStatus(nameof(Cancelled), RaffleStatusKind.Cancelled, "Cancelled by user");

    public static RaffleStatus Created { get; } = new RaffleStatus(nameof(Created), RaffleStatusKind.Created);

    public static RaffleStatus Scheduled { get; } =
      new RaffleStatus("Scheduled", RaffleStatusKind.InProgress);

    public static RaffleStatus DelayBeforeProcessing { get; } =
      new RaffleStatus("Delay Before Processing", RaffleStatusKind.InProgress);

    public static RaffleStatus SolvingCAPTCHA { get; } =
      new RaffleStatus("Solving CAPTCHA", RaffleStatusKind.InProgress);

    public static RaffleStatus ResolvingSMTPConfig { get; } =
      new RaffleStatus("Resolving SMTP Config", RaffleStatusKind.InProgress);

    public static RaffleStatus CantGetSMTPConfig { get; } =
      new RaffleStatus("Terminated", RaffleStatusKind.Failed, "Can't get SMTP config");

    public static RaffleStatus CatchAllNotSupported { get; } =
      new RaffleStatus("Terminated", RaffleStatusKind.Failed, "Catch all not supported");

    public static RaffleStatus GettingRaffleInfo { get; } =
      new RaffleStatus("Getting Raffle Info", RaffleStatusKind.InProgress);

    public static RaffleStatus VerifyingAccount { get; } =
      new RaffleStatus("Verifying account", RaffleStatusKind.InProgress);

    public static RaffleStatus LoggingIntoAccount { get; } =
      new RaffleStatus("Logging in to account", RaffleStatusKind.InProgress);

    public static RaffleStatus GettingAccountInfo { get; } =
      new RaffleStatus("Getting Account Info", RaffleStatusKind.InProgress);

    public static RaffleStatus Submitting { get; } =
      new RaffleStatus("Submitting entry", RaffleStatusKind.InProgress);

    public static RaffleStatus Waiting { get; } =
      new RaffleStatus("Simulating human time", RaffleStatusKind.InProgress);

    public RaffleStatus(string name, RaffleStatusKind kind, string description = null)
    {
      Name = name;
      Kind = kind;
      Description = description;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public RaffleStatusKind Kind { get; private set; }
    public bool IsRunning => Kind == RaffleStatusKind.InProgress;
    public bool IsSuccessful => Kind == RaffleStatusKind.Succeeded;

    public bool Equals(RaffleStatus other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }

      if (ReferenceEquals(this, other))
      {
        return true;
      }

      return Name == other.Name && Kind == other.Kind && Description == other.Description;
    }

    public override bool Equals(object obj)
    {
      return ReferenceEquals(this, obj) || obj is RaffleStatus other && Equals(other);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Name, (int) Kind);
    }

    public static bool operator ==(RaffleStatus left, RaffleStatus right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(RaffleStatus left, RaffleStatus right)
    {
      return !Equals(left, right);
    }

    public static RaffleStatus FailedWithCause(string message, string rootCause)
    {
      return new RaffleStatus(message, RaffleStatusKind.Failed, rootCause);
    }

    public static RaffleStatus UnknownError(string message)
    {
      return new RaffleStatus(nameof(Failed), RaffleStatusKind.Failed, message);
    }
  }
}