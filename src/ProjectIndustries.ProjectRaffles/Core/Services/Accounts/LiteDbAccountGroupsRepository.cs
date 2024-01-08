using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Accounts
{
  public class LiteDbAccountGroupsRepository : LiteDbRepositoryBase<AccountGroup>, IAccountGroupsRepository
  {
    public LiteDbAccountGroupsRepository(ILiteDatabase database)
      : base(database)
    {
    }
  }
}