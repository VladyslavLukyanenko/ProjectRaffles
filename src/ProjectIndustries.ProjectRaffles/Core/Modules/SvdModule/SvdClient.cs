using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public class SvdClient : ModuleHttpClientBase, ISvdClient
  {
    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.PostConfigure = httpClient =>
      {
        var deviceid = Guid.NewGuid().ToString();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "SVDApp/2002252213 CFNetwork/1188 Darwin/20.0.0");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("device-os", "I-iOS 14.0");
        httpClient.DefaultRequestHeaders.Add("app-version", "2.0.0");
        httpClient.DefaultRequestHeaders.Add("device-id", $"{deviceid}");
        httpClient.DefaultRequestHeaders.Add("bundle-version", "13");
        httpClient.DefaultRequestHeaders.Add("store-code", "en");
      };
    }

    public Task<string> LoginAsync()
    {
      //todo: make
      return Task.FromResult("");
    }
    public Task<string> GetUserInformationAsync()
    {
      //todo: GET to customer api and then parse, to get region ID
      return Task.FromResult("");
    }

    public async Task<string> GetShippingMethodAsync(Profile profile, Account account, string raffleId,
      CancellationToken ct)
    {
      //get shipping
      var shippingUrl = $"https://ms-api.sivasdescalzo.com/api/raffles/{raffleId}/estimate-shipping";

      //use ShippingDto
      var shipping = new
      {
        address = new
        {
          city = profile.ShippingAddress.City,
          country_id = profile.ShippingAddress.CountryId,
          custom_attributes = new { },
          firstname = profile.ShippingAddress.FirstName,
          lastname = profile.ShippingAddress.LastName,
          postcode = profile.ShippingAddress.ZipCode,
          region = profile.ShippingAddress.ProvinceCode,
          region_id = "",
          street = new[] {profile.ShippingAddress.AddressLine1},
          telephone = profile.ShippingAddress.PhoneNumber
        }
      };

      var shippingSerialize = JsonConvert.SerializeObject(shipping);
      var shippingData = new StringContent(shippingSerialize, Encoding.UTF8, "application/json");

      var message = new HttpRequestMessage(HttpMethod.Post, shippingUrl)
      {
        Headers =
        {
          Authorization = new AuthenticationHeaderValue("Bearer", account.AccessToken)
        },
        Content = shippingData
      };

      var shippingresult = await HttpClient.SendAsync(message, ct);
      var shippingresultstring = await shippingresult.Content.ReadAsStringAsync();

      var shippingmethodArray = JArray.Parse(shippingresultstring);
      dynamic shippingmethodparsed = JObject.Parse(shippingmethodArray[0].ToString());
      string shippingMethod = shippingmethodparsed.method_code;
      string shippingPrice = shippingmethodparsed.amount;

      return shippingMethod;
    }

    public async Task<string> GetAuthorizationFingerprintAsync(Account account, CancellationToken ct)
    {
      var message = new HttpRequestMessage(HttpMethod.Get, "https://ms-api.sivasdescalzo.com/api/carts/payments/token")
      {
        Headers =
        {
          Authorization = new AuthenticationHeaderValue("Bearer", account.AccessToken)
        }
      };

      var tokenResponse = await HttpClient.SendAsync(message, ct);
      var tokenresult = await tokenResponse.Content.ReadAsStringAsync();
      dynamic stuff = JObject.Parse(tokenresult);
      string authtoken = stuff.token;
      var b64data = Convert.FromBase64String(authtoken);
      var decodedString = Encoding.UTF8.GetString(b64data);
      dynamic paymenttoken = JObject.Parse(decodedString);

      return paymenttoken.authorizationFingerprint;
    }

    public async Task<bool> SubmitRaffleAsync(SvdRaffleSubmitPayload payload, CancellationToken ct)
    {
      var raffleurl = $"https://ms-api.sivasdescalzo.com/api/raffles/{payload.RaffleId}";
      // _httpClient.DefaultRequestHeaders.Clear();
      // _httpClient.DefaultRequestHeaders.Add("store-code", "en");
      // _httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {loginToken}");
      // _httpClient.DefaultRequestHeaders.Add("device-id", $"{deviceid}");
      // _httpClient.DefaultRequestHeaders.Add("app-version", "2.0.0");
      // _httpClient.DefaultRequestHeaders.Add("bundle-version", "13");
      // _httpClient.DefaultRequestHeaders.Add("device-os", "A-Android 10");
      // _httpClient.DefaultRequestHeaders.Add("user-agent", "okhttp/3.12.1");

      // better to use RaffleRoot
      var raffleroot = new
      {
        participation = new
        {
          billing_city = payload.Profile.BillingAddress.City,
          billing_country_id = payload.Profile.BillingAddress.CountryId,
          billing_firstname = payload.Profile.BillingAddress.FirstName,
          billing_lastname = payload.Profile.BillingAddress.LastName,
          billing_postcode = payload.Profile.BillingAddress.ZipCode,
          billing_region = payload.Profile.BillingAddress.ProvinceCode,
          billing_region_id = "",
          billing_street = payload.Profile.BillingAddress.AddressLine1,
          billing_telephone = payload.Profile.BillingAddress.PhoneNumber,
          payment_data = payload.PaymentMethod,
          product_id = payload.RaffleId,
          product_option_id = payload.OptionId,
          shipping_city = payload.Profile.ShippingAddress.City,
          shipping_country_id = payload.Profile.ShippingAddress.CountryId,
          shipping_firstname = payload.Profile.ShippingAddress.FirstName,
          shipping_lastname = payload.Profile.ShippingAddress.LastName,
          shipping_method = payload.ShippingMethod,
          shipping_postcode = payload.Profile.ShippingAddress.ZipCode,
          shipping_region = payload.Profile.ShippingAddress.ProvinceCode,
          shipping_region_id = "",
          shipping_street = payload.Profile.ShippingAddress.AddressLine1,
          shipping_telephone = payload.Profile.ShippingAddress.PhoneNumber,
          size_group = "",
          store_group_id = 1
        }
      };

      var raffleRegistration = JsonConvert.SerializeObject(raffleroot);
      var raffleRegistrationData = new StringContent(raffleRegistration, Encoding.UTF8, "application/json");


      var message = new HttpRequestMessage(HttpMethod.Post, raffleurl)
      {
        Headers =
        {
          Authorization = new AuthenticationHeaderValue("Bearer", payload.Account.AccessToken)
        },
        Content = raffleRegistrationData
      };

      var response = await HttpClient.SendAsync(message, ct);
      return response.IsSuccessStatusCode;
    }
  }
}