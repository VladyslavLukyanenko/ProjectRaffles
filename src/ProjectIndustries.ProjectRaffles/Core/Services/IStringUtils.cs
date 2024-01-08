using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface IStringUtils
    {
        Task<string> GenerateRandomStringAsync(int length);
    }
}