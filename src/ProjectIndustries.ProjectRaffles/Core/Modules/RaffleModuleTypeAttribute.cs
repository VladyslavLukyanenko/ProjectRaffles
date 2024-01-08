using System;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RaffleModuleTypeAttribute : Attribute
  {
    public RaffleModuleTypeAttribute(RaffleModuleType type)
    {
      Type = type;
    }

    public RaffleModuleType Type { get; }
  }
}