using System;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RaffleModuleNameAttribute : Attribute
  {
    public RaffleModuleNameAttribute(string name)
    {
      Name = name;
    }

    public string Name { get; }
  }
}