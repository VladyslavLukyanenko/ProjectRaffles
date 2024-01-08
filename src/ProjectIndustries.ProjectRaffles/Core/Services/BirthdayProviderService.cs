using System;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public class BirthdayProviderService : IBirthdayProviderService
    {
        private readonly Random _rnd = new Random((int) DateTime.Now.Ticks);

        public Task<int> GetDate()
        {
            var date = _rnd.Next(1,25);
            return Task.FromResult(date);
        }

        public Task<int> GetMonth()
        {
            var month = _rnd.Next(1, 12);
            return Task.FromResult(month);
        }

        public Task<int> GetYear()
        {
            var year = _rnd.Next(1985, 2001);
            return Task.FromResult(year);
        }

        public Task<int> GenerateAge()
        {
            var age = _rnd.Next(18, 25);
            return Task.FromResult(age);
        }

        public Task<string> GenerateDob()
        {
            var dateInt = GetDate().Result;
            var month = GetMonth().Result;
            var year = GetYear().Result;

            var dateString = dateInt.ToString();
            if (dateInt < 10)
            {
                dateString = $"0{dateInt}";
            }

            var dob = $"{dateString}/{month}/{year}";

            return Task.FromResult(dob);
        }
    }
}