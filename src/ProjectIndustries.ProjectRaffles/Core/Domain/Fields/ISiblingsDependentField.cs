using System.Collections.Generic;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields
{
  public interface ISiblingsDependentField
  {
    void Consume(IEnumerable<Field> siblings);
  }
}