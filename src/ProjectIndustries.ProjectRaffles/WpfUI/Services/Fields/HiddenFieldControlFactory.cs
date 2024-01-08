using System.Windows;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class HiddenFieldControlFactory : IFieldControlFactory
  {
    public bool IsSupported(Field field) => field is HiddenField;

    public UIElement Create(Field field)
    {
      return null;
    }
  }
}