using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule
{
  public class StayRootedClient : ModuleHttpClientBase, IStayRootedClient
  {
    private readonly ICountriesService _countriesService;

    public StayRootedClient(ICountriesService countriesService)
    {
      _countriesService = countriesService;
    }

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
      };
    }

    public async Task<string> GetCustomerIdAsync(StayRootedValidationPayload payload, CancellationToken ct)
    {
      var validateEmailContent = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"email", payload.Email},
        {"product_id", payload.ProductId}
      });
      var emailValidationUrl = "https://rooted-draw.herokuapp.com/customers/validateEmail";
      var validateEmail = await HttpClient.PostAsync(emailValidationUrl, validateEmailContent, ct);
      var validateEmailBody = await validateEmail.ReadStringResultOrFailAsync("Can't validate email", ct);

      var validateEmailNoArray = validateEmailBody.Replace("[", "").Replace("]", "");
      dynamic validateEmailParsed = JObject.Parse(validateEmailNoArray);
      string customerId = validateEmailParsed.customers.id;

      return customerId;
    }

    public async Task<string> StartEntryProcessAsync(StayRootedEntryPayload payload, CancellationToken ct)
    {
      var country = _countriesService.GetCountryName(payload.Profile.ShippingAddress.CountryId)
        .Replace(" (" + payload.Profile.ShippingAddress.CountryId + ")", "");
      
      var entryContent = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"shipping_first_name", payload.Profile.ShippingAddress.FirstName},
        {"shipping_last_name", payload.Profile.ShippingAddress.LastName},
        {"customer_id", payload.CustomerId},
        {"variant_id", payload.VariantId},
        {"street_address", payload.Profile.ShippingAddress.AddressLine1},
        {"city", payload.Profile.ShippingAddress.City},
        {"zip", payload.Profile.ShippingAddress.ZipCode},
        {
          "state", payload.Profile.ShippingAddress.ProvinceCode
        }, //todo: make check, if users profile does NOT have a state, revert to "none"
        {"phone", payload.Profile.ShippingAddress.PhoneNumber},
        {"country", country},
        {"delivery_method", "online"}
      });

      var entryUrl = "https://rooted-draw.herokuapp.com/draws/entries/new";
      var entryPost = await HttpClient.PostAsync(entryUrl, entryContent, ct);
      var entryPostBody = await entryPost.ReadStringResultOrFailAsync("Error on creating entry", ct);
      
      dynamic entryIdParser = JObject.Parse(entryPostBody);
      string entryId = entryIdParser.id;

      return entryId;
    }

    public async Task<bool> SubmitAsync(StayRootedFinalPayload payload, CancellationToken ct)
    {
      var finalContent = new FormUrlEncodedContent(new Dictionary<string, string>
      {
        {"checkout_token", payload.PaymentToken},
        {"entry_id", payload.EntryId}
      });

      var url = "https://rooted-draw.herokuapp.com/draws/entries/checkout";
      var post = await HttpClient.PostAsync(url, finalContent, ct);
      var body = await post.ReadStringResultOrFailAsync("Error on submission", ct);

      return body.Contains("Entry successfully finalized");
    }
  }
}