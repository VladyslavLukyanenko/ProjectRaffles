using System;
using System.Net;
using System.Net.Http;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public class HttpClientBuilder : IHttpClientBuilder
  {
    private HttpClientHandler _clientHandler = new HttpClientHandler();
    private readonly HttpClientOptions _options = new HttpClientOptions();

    public HttpClientBuilder()
    {
      
    }

    public IHttpClientBuilder WithProxy(IWebProxy proxy)
    {
      _clientHandler.Proxy = proxy;
      _clientHandler.UseProxy = proxy != null;
      return this;
    }

    public IHttpClientBuilder WithConfiguration(Action<HttpClientOptions> configurer)
    {
      configurer?.Invoke(_options);
      _clientHandler.AllowAutoRedirect = _options.AllowAutoRedirect;
      _clientHandler.UseCookies = _options.CookieContainer != null;
      if (_clientHandler.UseCookies)
      {
        _clientHandler.CookieContainer = _options.CookieContainer;
      }

      return this;
    }

    public HttpClient Build()
    {
      var client = new HttpClient(_clientHandler);
      _options.PostConfigure?.Invoke(client);

      return client;
    }

    public HttpClientHandler BuildHandler()
    {
      return _clientHandler;
    }
    
    public HttpClientHandler BuildHandlerWithProxy(Proxy proxy)
    {
      var newProxy = proxy.ToWebProxy();
      WithProxy(newProxy);
      return _clientHandler;
    }
  }
}