using System.Reflection;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Proxies
{
  [Obfuscation(Feature = "renaming", ApplyToMembers = true)]
  public class CsvProxyData
  {
    public string GroupName { get; set; }

    public string Password { get; set; }
    public string Username { get; set; }
    public string Url { get; set; }
  }
}