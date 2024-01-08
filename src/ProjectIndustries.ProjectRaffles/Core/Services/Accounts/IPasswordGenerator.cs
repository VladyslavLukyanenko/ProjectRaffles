namespace ProjectIndustries.ProjectRaffles.Core.Services.Accounts
{
  public interface IPasswordGenerator
  {
    string Generate(int len);
  }
}