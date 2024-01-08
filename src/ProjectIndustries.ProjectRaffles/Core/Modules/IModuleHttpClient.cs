using System.Net.Http.Headers;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public interface IModuleHttpClient
  {
    void Initialize(IHttpClientBuilder builder);
    HttpRequestHeaders DefaultHeaders { get; }
  }
}