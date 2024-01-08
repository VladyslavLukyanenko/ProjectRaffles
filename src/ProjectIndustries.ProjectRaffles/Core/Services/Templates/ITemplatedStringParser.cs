using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public interface ITemplatedStringParser
  {
    IEnumerable<TemplateToken> Parse(string input);
  }
}