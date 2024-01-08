namespace ProjectIndustries.ProjectRaffles.Core.Domain.Emails
{
  public class ImapCredentials
  {
    public ImapCredentials(Email email)
    {
      Login = email.Value;
      Password = email.Password;
      ImapConfig = email.ImapConfig;
    }

    public string Login { get; set; }
    public string Password { get; set; }
    public ImapConfig ImapConfig { get; set; }
  }
}