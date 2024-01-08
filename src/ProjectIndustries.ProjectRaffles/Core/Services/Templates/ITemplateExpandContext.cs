namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public interface ITemplateExpandContext
  {
    ITemplateVariableExpander GetExpander(string varName);
  }
}