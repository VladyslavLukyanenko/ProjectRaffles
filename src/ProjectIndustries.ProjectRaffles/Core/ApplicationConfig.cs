using ProjectIndustries.ProjectRaffles.Core.Clients;

namespace ProjectIndustries.ProjectRaffles.Core
{
  public class ApplicationConfig
  {
    public string StorageLocation { get; set; }
    public ConnectionStringsConfig ConnectionStrings { get; set; } = new ConnectionStringsConfig();
    public SecurityConfig Security { get; set; } = new SecurityConfig();
    public ProjectIndustriesApiConfig ProjectIndustriesApi { get; set; } = new ProjectIndustriesApiConfig();
  }
}