using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AlumniModule
{
  public class AlumniClient : ModuleHttpClientBase, IAlumniClient
  {
    public async Task<AlumniParsedRaffle> ParseRaffleAsync(CancellationToken ct)
    {
      var url = "https://www.alumniofny.com/pages/raffles";
      var web = new HtmlWeb();
      var doc = await web.LoadFromWebAsync(url, ct);
      
      var formId = doc.DocumentNode.SelectSingleNode("//input[@name='form_id']").GetAttributeValue("value", "");
      var containerId = doc.DocumentNode.SelectSingleNode("//input[@name='container_id']")
        .GetAttributeValue("value", "");
      
      return new AlumniParsedRaffle(formId, containerId);
    }
    
    public async Task<bool> SubmitAsync(AlumniSubmitPayload payload, CancellationToken ct)
    {
      var content = new MultipartFormDataContent("----WebKitFormBoundary" + Guid.NewGuid())
      {
        {new StringContent(""), "url"},
        {new StringContent(payload.ParsedRaffle.ContainerId), "container_id"},
        {new StringContent(payload.ParsedRaffle.FormId), "form_id"},
        {new StringContent("alumni-of-new-york.myshopify.com"), "form_domain"},
        {new StringContent("100"), "uploadeLimit"},
        {new StringContent(""), "allowefiletype"},
        {new StringContent(""), "conditionallogic"},
        {new StringContent(payload.Profile.ShippingAddress.FirstName), "first-name"},
        {new StringContent(payload.Profile.ShippingAddress.LastName), "last-1579735867875"},
        {new StringContent(payload.Profile.ShippingAddress.AddressLine1), "aaa-customer-address"},
        {new StringContent(payload.Profile.ShippingAddress.AddressLine2), "aaa-customer-address1"},
        {new StringContent(payload.Profile.ShippingAddress.City), "aaa-customer-city"},
        {new StringContent("United States"), "aaa-form-builder-country"},
        {new StringContent("state"), "aaa-customer-state"},
        {new StringContent(payload.Profile.ShippingAddress.ZipCode), "aaa-customer-zip-code"},
        {new StringContent(payload.Profile.ShippingAddress.PhoneNumber), "aaa-customer-phone-numbe"},
        {new StringContent(payload.Email), "replyemail"},
        {new StringContent(payload.SizeValue), "select-1579734716807"},
        {new StringContent(""), "button-1481029333721"},
        {new StringContent("on"), "aaasubscribenewsletter"} //todo: check if buttons change ID Value
      };

      var endpoint = "https://www.alumniofny.com/pages/raffles";
      var raffleResponse = await HttpClient.PostAsync(endpoint, content, ct);

      return raffleResponse.IsSuccessStatusCode;
    }
  }
}