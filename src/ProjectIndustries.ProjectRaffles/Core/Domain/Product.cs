using System;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class Product : IEntity
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Picture { get; set; }
  }
}