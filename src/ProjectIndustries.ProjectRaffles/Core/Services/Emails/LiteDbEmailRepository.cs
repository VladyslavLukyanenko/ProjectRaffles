using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public class LiteDbEmailRepository : LiteDbRepositoryBase<Email>, IEmailRepository
  {
    public LiteDbEmailRepository(ILiteDatabase database)
      : base(database)
    {
    }

    public async Task RemoveAllAsync(CancellationToken ct = default)
    {
      Collection.DeleteAll();
      await InitializeAsync(ct);
    }
  }
}