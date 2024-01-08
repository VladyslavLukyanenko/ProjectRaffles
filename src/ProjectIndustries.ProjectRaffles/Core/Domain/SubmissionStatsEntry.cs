namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class SubmissionStatsEntry
  {
    public string ProviderName { get; set; }
    public string ProductName { get; set; }
    public string ProfileName { get; set; }
    public string Sizes { get; set; }
    public bool IsSuccessful { get; set; }
  }
}