using System;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class User
  {
    public User(long id, string username, long discriminator, DateTimeOffset? licenseExpiryDate, string avatar)
    {
      Id = id;
      Username = username;
      Discriminator = discriminator;
      StatusText = licenseExpiryDate == null
        ? "Lifetime"
        : $"Renewal: {licenseExpiryDate.Value:MM-dd-yyyy}";
      Avatar = avatar;
      FullUserName = $"{username}#{discriminator.ToString().PadLeft(4, '0')}";
    }

    public long Id { get; }
    public string Username { get; }
    public string FullUserName { get; set; }
    public long Discriminator { get; }
    public string StatusText { get; }
    public string Avatar { get; }
  }
}