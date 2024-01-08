using System;
using System.Text.RegularExpressions;
using LiteDB;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Emails
{
  public class Email : Entity
  {
    private static readonly ICatchAllEmailMaterializer FallbackEmailMaterializer =
      new GuidLikeCatchAllEmailMaterializer();

    public static ICatchAllEmailMaterializer Materializer { get; set; }

    public static readonly Regex CatchAllRegex =
      new Regex(@"^@[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$",
        RegexOptions.Compiled);

    private static readonly Regex EmailRegex = new Regex(
      @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
      RegexOptions.Compiled);

    public Email()
    {
    }

    [BsonCtor]
    public Email(Guid id, string value, bool isCatchAll, string password, ImapConfig imapConfig, SmtpConfig smtpConfig)
      : base(id)
    {
      Value = value;
      IsCatchAll = isCatchAll;
      Password = password;
      ImapConfig = imapConfig;
      SmtpConfig = smtpConfig;
    }

    public bool CanBeTracked() =>
      !IsCatchAll && ImapConfig != null || MasterEmail != null && MasterEmail.CanBeTracked();

    public Email(string value, bool isCatchAll, string password)
    {
      value = value?.Trim();
      if (string.IsNullOrEmpty(value))
      {
        throw new ArgumentNullException(nameof(value));
      }

      Value = value;
      IsCatchAll = isCatchAll;
      Password = password?.Trim();
    }

    public string Value { get; private set; }
    public bool IsCatchAll { get; private set; }
    public string Password { get; private set; }
    public ImapConfig ImapConfig { get; set; }
    public SmtpConfig SmtpConfig { get; set; }

    public Email MasterEmail { get; set; }

    public ImapCredentials GetImapCredentials() =>
      IsCatchAll
        ? MasterEmail?.GetImapCredentials()
        : new ImapCredentials(this);

    public Email MaterializeEmail()
    {
      if (IsCatchAll)
      {
        return new Email
        {
          IsCatchAll = true,
          Value = Materializer?.Materialize(Value) ?? FallbackEmailMaterializer.Materialize(Value),
          MasterEmail = MasterEmail
        };
      }

      return this;
    }

    public static bool TryParse(string raw, out Email email)
    {
      var tokens = raw.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
      if (tokens.Length == 0 || tokens.Length > 2)
      {
        email = null;
        return false;
      }

      var rawEmail = tokens[0];
      var password = tokens.Length == 2 ? tokens[1] : null;
      var isCatchAll = CatchAllRegex.IsMatch(rawEmail);
      if (!isCatchAll && !EmailRegex.IsMatch(rawEmail))
      {
        email = null;
        return false;
      }

      email = new Email(rawEmail, isCatchAll, password);

      return true;
    }

    public string GetDomainName()
    {
      var idx = Value.IndexOf('@') + 1;

      return Value[idx..];
    }

    public static bool TryCreateRegular(string emailAddress, string password, out Email email)
    {
      if (!EmailRegex.IsMatch(emailAddress))
      {
        email = null;
        return false;
      }

      email = new Email(emailAddress, false, password);
      return true;
    }

    public override string ToString()
    {
      return $"{nameof(Email)}({Value})";
    }

    private class GuidLikeCatchAllEmailMaterializer : ICatchAllEmailMaterializer
    {
      public string Materialize(string catchallEmail) => Guid.NewGuid().ToString("N") + catchallEmail;
    }
  }
}