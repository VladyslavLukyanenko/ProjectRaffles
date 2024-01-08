using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public interface IRaffleModuleExpandContext : ITemplateExpandContext
  {
    IRaffleExecutionContext ExecutionContext { get; }
    IEnumerable<Field<string>> Fields { get; }
  }
}