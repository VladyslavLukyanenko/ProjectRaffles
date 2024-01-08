using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Proxies
{
  public class LiteDbProxyGroupsRepository : LiteDbRepositoryBase<ProxyGroup>, IProxyGroupsRepository
  {
    public LiteDbProxyGroupsRepository(ILiteDatabase database)
      : base(database)
    {
    }
  }
}