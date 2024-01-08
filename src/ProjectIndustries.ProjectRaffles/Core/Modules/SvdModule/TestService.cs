using System;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public class TestService : ITestService
  {
    public Task<TestPayload> RegularCall(string userName, int age)
    {
      return CallResultWillBeCached(userName, age);
    }

    public async Task<TestPayload> CallResultWillBeCached(string userName, int age)
    {
      await Task.Delay(TimeSpan.FromSeconds(2));
      return new TestPayload
      {
        Prop1 = userName,
        Prop2 = DateTime.Now,
        Prop3 = age
      };
    }
  }
}