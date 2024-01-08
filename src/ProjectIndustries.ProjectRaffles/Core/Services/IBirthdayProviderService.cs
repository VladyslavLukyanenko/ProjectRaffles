using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface IBirthdayProviderService
    {
        public Task<int> GetDate();
        public Task<int> GetMonth();
        public Task<int> GetYear();
        public Task<int> GenerateAge();
        public Task<string> GenerateDob();
    }
}