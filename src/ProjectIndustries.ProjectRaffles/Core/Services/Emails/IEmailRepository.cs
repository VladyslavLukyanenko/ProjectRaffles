using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Emails
{
  public interface IEmailRepository : IRepository<Email>
  {
    Task RemoveAllAsync(CancellationToken ct = default);
  }
}