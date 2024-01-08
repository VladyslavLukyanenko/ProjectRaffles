using System.Net;
using AngleSharp.Html.Dom;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp.Html
{
  public interface IMailChimpHtmlFormParser
  {
    MailChimpParseResult Parse(IHtmlFormElement form, CookieContainer cookieContainer);
  }
}