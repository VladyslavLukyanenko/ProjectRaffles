using System.Windows;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public interface IFieldControlFactory
  {
    bool IsSupported(Field field);
    UIElement Create(Field field);
  }
}