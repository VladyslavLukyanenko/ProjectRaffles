using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public class MailchimpService : IMailchimpService
    {
        private readonly Random _rnd = new Random((int) DateTime.Now.Ticks);

        public Task<string> FindBotField(string html)
        {
            var botFieldPattern = @"name=""b_.*"" tab";
            var botFieldRegex = new Regex(botFieldPattern);
            var botField = botFieldRegex.Match(html).ToString().Replace(@"name=""","").Replace(@""" tab","");

            return Task.FromResult(botField);
        }

        public Task<string> GetUnixTime()
        {
            var now = DateTimeOffset.UtcNow;
            var unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
            var unixTimeString = unixTimeMilliseconds.ToString();

            return Task.FromResult(unixTimeString);
        }

        public Task<string> GeneratejQueryId()
        {
            var number = _rnd.Next();

            var unixTimeMilliseconds =  GetUnixTime().Result;

            var mailString = "jQuery1900" + number + "_" + unixTimeMilliseconds;

            return Task.FromResult(mailString);
        }
    }
}