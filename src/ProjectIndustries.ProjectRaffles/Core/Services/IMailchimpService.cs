using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface IMailchimpService
    {
        Task<string> FindBotField(string html);
        Task<string> GetUnixTime();
        Task<string> GeneratejQueryId();
    }
}