using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.InstagramAccountGenerator
{
    public class InstagramAccountGeneratorClient : IInstagramAccountGeneratorClient
    {
        private readonly IHttpClientBuilder _builder;
        private HttpClientHandler _handler;
        private readonly Random _random = new Random();
        private IInstaApi InstaApi;
        

        public InstagramAccountGeneratorClient(IHttpClientBuilder builder)
        {
            _builder = builder;
        }

        public void GenerateInstaApiHandler(string email, Proxy proxy)
        {
            _handler = _builder.BuildHandlerWithProxy(proxy);

            var userSession = new UserSessionData
            {
                UserName = email,
                Password = "ProjectRaffles!1)3!"
            };
            
            InstaApi = InstaApiBuilder.CreateBuilder()
                .UseHttpClientHandler(_handler)
                .SetUser(userSession)
                .Build();
        }

        public async Task CheckEmailAvailability(string email)
        {
            var checkEmail = await InstaApi.CheckEmailAsync(email);
            
            if(!checkEmail.Succeeded) throw new RaffleFailedException("email in use", "Email already used"); 
        }

        public async Task<bool> CreateAccount(AddressFields nameValues, string email, CancellationToken ct)
        {
            var username = nameValues.FirstName.Value + nameValues.LastName.Value + _random.Next(1, 9999);
            var create = await InstaApi.CreateNewAccountAsync(username, "ProjectRaffles!1)3!", email, nameValues.FirstName.Value);

            if (create.Info.NeedsChallenge)
            {
                var challenge = await InstaApi.GetChallengeRequireVerifyMethodAsync();
            }
            
            if(!create.Succeeded) throw new RaffleFailedException("error on creation, " + create.Info.Message, "Error on creation for email " + email);

            return create.Succeeded;
        }
    }
}