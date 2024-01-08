using System;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RaffleAccountGeneratorAttribute : Attribute
  {
    public RaffleAccountGeneratorAttribute(Type generatorType)
    {
      GeneratorType = generatorType;
    }

    public Type GeneratorType { get; }
  }
}