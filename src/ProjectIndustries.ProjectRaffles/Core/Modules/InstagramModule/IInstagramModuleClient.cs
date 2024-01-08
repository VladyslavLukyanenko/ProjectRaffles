using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Caches;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.InstagramModule
{
    [EnableCaching]
    public interface IInstagramModuleClient : IModuleHttpClient
    {
        [CacheOutput]
        Task<string> GetMediaPk(string url, CancellationToken ct);

        Task LoginAsync(Account account, CancellationToken ct);
        
        Task<bool> PostComment(string url, string comment, CancellationToken ct);
        
        Task<bool> PostFollow(string userId, CancellationToken ct);
        
        Task<bool> PostLike(string url, CancellationToken ct);

        Task<bool> PostStoryAsync(long mediaPk, CancellationToken ct);

        Task<bool> PostDirectMessage(string userToDm, string message);
    }
}