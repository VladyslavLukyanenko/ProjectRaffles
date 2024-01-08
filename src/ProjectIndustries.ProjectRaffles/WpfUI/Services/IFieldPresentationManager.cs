using System.Collections.Generic;
using System.Windows.Controls;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public interface IFieldPresentationManager
  {
    void DisplayFields(StackPanel surface, IEnumerable<Field> fields, int maxItemsPerRow = 4);
  }
}