using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.CustomLists
{
  public class LiteDbCustomListRepository : LiteDbRepositoryBase<CustomList>, ICustomListRepository
  {
    public LiteDbCustomListRepository(ILiteDatabase database)
      : base(database)
    {
    }
  }
}