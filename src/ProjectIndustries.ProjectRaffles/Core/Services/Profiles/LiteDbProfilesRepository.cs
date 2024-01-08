using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Profiles
{
  public class LiteDbProfilesRepository : LiteDbRepositoryBase<Profile>, IProfilesRepository
  {
    public LiteDbProfilesRepository(ILiteDatabase database)
      : base(database)
    {
    }
  }
}