using System.Net;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
    public class InstaHttpClientBase : ModuleHttpClientBase
    {
        public override void Initialize(IHttpClientBuilder builder)
        {
            HttpClientHandler = builder.BuildHandler();
        }
    }
}