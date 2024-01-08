using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  [EnableCaching]
  public interface ITestService
  {
    Task<TestPayload> RegularCall(string userName, int age);

    [CacheOutput]
    Task<TestPayload> CallResultWillBeCached(string userName, int age);
  }
}