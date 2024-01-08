using System.Collections.Generic;
using System.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public class DefaultRaffleModuleExpandContextFactory : IRaffleModuleExpandContextFactory
  {
    private readonly ITemplateExpandContext _templateExpandContext;

    public DefaultRaffleModuleExpandContextFactory(ITemplateExpandContext templateExpandContext)
    {
      _templateExpandContext = templateExpandContext;
    }

    public IRaffleModuleExpandContext Create(IRaffleModule module, IRaffleExecutionContext context)
    {
      return new DefaultRaffleModuleExpandContext(module, context, _templateExpandContext);
    }

    private class DefaultRaffleModuleExpandContext : IRaffleModuleExpandContext
    {
      private readonly IRaffleModule _module;
      private readonly ITemplateExpandContext _impl;

      public DefaultRaffleModuleExpandContext(IRaffleModule module, IRaffleExecutionContext executionContext,
        ITemplateExpandContext impl)
      {
        _module = module;
        _impl = impl;
        ExecutionContext = executionContext;
      }

      public ITemplateVariableExpander GetExpander(string varName) => _impl.GetExpander(varName);

      public IRaffleExecutionContext ExecutionContext { get; }
      public IEnumerable<Field<string>> Fields => _module.AdditionalFields.OfType<Field<string>>();
    }
  }
}