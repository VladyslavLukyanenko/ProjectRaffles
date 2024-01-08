using System;
using System.Net;
using System.Net.Http;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public class HttpClientOptions
  {
    public CookieContainer CookieContainer { get; set; }
    public bool AllowAutoRedirect { get; set; } = true;
    public Action<HttpClient> PostConfigure { get; set; }
  }
}