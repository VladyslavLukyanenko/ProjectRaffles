using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AwolModule
{
  public class AwolClient : ModuleHttpClientBase, IAwolClient
  {
    public async Task<AwolParsedRaffle> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
    {
      var web = new HtmlWeb();
      var doc = await web.LoadFromWebAsync(raffleUrl, ct);
      var formBuildId = doc.DocumentNode.SelectSingleNode("//input[@name='form_build_id']")
        .GetAttributeValue("value", "");
      var formId = doc.DocumentNode.SelectSingleNode("//input[@name='form_id']").GetAttributeValue("value", "");

      return new AwolParsedRaffle(formId, formBuildId);
    }

    public async Task<bool> SubmitAsync(AwolSubmitPayload payload, CancellationToken ct)
    {
      var fullname = payload.Profile.FirstName + " " + payload.Profile.LastName;

      var content = new MultipartFormDataContent("----WebKitFormBoundary" + Guid.NewGuid())
      {
        {new StringContent(fullname), "submitted[name]"},
        {new StringContent(payload.Email), "submitted[e_mail][mailchimp_email_address]"},
        {new StringContent(payload.SizeValue), "submitted[shoe_size]"},
        {new StringContent(payload.PickupLocation), "submitted[location]"},
        {new StringContent("1"), "submitted[terms][1]"},
        {new StringContent(payload.Profile.AddressLine1), "submitted[address]"},
        {new StringContent(""), "details[sid]"},
        {new StringContent("1"), "details[page_num]"},
        {new StringContent("1"), "details[page_count]"},
        {new StringContent("0"), "details[finished]"},
        {new StringContent(payload.ParsedRaffle.FormBuildId), "form_build_id"},
        {new StringContent(payload.ParsedRaffle.FormId), "form_id"},
        {new StringContent("Submit"), "op"}
      };

      var raffleResponse = await HttpClient.PostAsync(payload.RaffleUrl, content, ct);
      if(!raffleResponse.IsSuccessStatusCode) await raffleResponse.FailWithRootCauseAsync("Error on submission",ct);

      return raffleResponse.IsSuccessStatusCode;
    }
  }
}