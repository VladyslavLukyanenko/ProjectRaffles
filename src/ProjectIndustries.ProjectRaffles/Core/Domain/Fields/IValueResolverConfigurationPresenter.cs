using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields
{
  public interface IValueResolverConfigurationPresenter
  {
    Task ShowConfigurationWindowAsync(string title, params Field[] configurationFields);
  }
}