using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  public interface ICaptchaRepository : IRepository<CaptchaProvider>
  {
    Task<CaptchaProvider> GetProviderByName(string name, CancellationToken ct = default);
  }
}