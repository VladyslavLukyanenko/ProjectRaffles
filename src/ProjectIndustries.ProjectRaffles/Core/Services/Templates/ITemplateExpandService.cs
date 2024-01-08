namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public interface ITemplateExpandService
  {
    string Expand(string input, ITemplateExpandContext context);
  }
}