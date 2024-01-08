using System;
using System.Net;
using System.Net.Http;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public interface IHttpClientBuilder
  {
    IHttpClientBuilder WithProxy(IWebProxy proxy);
    IHttpClientBuilder WithConfiguration(Action<HttpClientOptions> configurer);
    HttpClient Build();
    HttpClientHandler BuildHandler();
    HttpClientHandler BuildHandlerWithProxy(Proxy proxy);
  }
}