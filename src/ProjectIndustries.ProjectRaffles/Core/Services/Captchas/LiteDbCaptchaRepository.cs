using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  public class LiteDbCaptchaRepository : LiteDbRepositoryBase<CaptchaProvider>, ICaptchaRepository
  {
    public LiteDbCaptchaRepository(ILiteDatabase database)
      : base(database)
    {
    }

    public Task<CaptchaProvider> GetProviderByName(string name, CancellationToken ct = default)
    {
      var item = Cache.Items.FirstOrDefault(_ => _.ProviderName == name);
      return Task.FromResult(item);
    }
  }
}