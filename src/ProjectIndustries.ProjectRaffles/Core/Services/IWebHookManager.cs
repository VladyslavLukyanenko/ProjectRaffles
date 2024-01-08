using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IWebHookManager
  {
    void EnqueueWebhook(PendingRaffleTask task);
    void Spawn();
    Task<bool> TestWebhook();
  }
}