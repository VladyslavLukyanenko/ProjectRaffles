using System.Net.Http;
using System.Net.Http.Headers;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public abstract class ModuleHttpClientBase : IModuleHttpClient
  {
    protected HttpClient HttpClient;
    protected HttpClientHandler HttpClientHandler;

    public virtual void Initialize(IHttpClientBuilder builder)
    {
      HttpClient = builder.WithConfiguration(ConfigureHttpClient)
        .Build();
    }

    public HttpRequestHeaders DefaultHeaders => HttpClient.DefaultRequestHeaders;

    protected virtual void ConfigureHttpClient(HttpClientOptions options)
    {
    }
  }
}