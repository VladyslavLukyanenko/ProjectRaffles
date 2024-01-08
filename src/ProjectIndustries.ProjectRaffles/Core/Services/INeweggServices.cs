using System;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface INeweggServices
    {
        Task<string> GenerateRandomString(int length);

        Task<string> GenerateRandomDigitString(int length);

        Task<AccertifyJsonClass> GetPayload(string signupUrl);

        Task<string> AccertifyEvents(string signupUrl, long pageId, string sessionId);

        Task<string> EncryptPayload(string stringDataToEncrypt, string stringPublicKey);

        Task<long> NextLong(Random random, long min, long max);
        
        
    }
}