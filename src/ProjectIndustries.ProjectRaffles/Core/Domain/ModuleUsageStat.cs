using System;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class ModuleUsageStat : Entity
  {
    public string ModuleComputedId { get; set; }
    public Guid ProfileId { get; set; }
    public Guid? ProxyId { get; set; }
  }
}