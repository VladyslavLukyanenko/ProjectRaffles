using System.Threading.Tasks;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields
{
  public interface IValueChangePostProcessable
  {
    Task PostProcessAsync(IReadonlyDependencyResolver dependencyResolver);
  }
}