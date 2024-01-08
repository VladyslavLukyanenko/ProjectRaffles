using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.FootpatrolModule
{
  public class FootpatrolClient : ModuleHttpClientBase, IFootpatrolClient
  {
    private readonly IBirthdayProviderService _birthdayProviderService;

    public FootpatrolClient(IBirthdayProviderService birthdayProviderService)
    {
      _birthdayProviderService = birthdayProviderService;
    }

    public async Task<string> SizeParserAsync(string raffleurl, string size, CancellationToken ct)
    {
      var raffleId =
        raffleurl.Substring(raffleurl.Length -
                            3); //JDSports raffles are currently only 3 characters long, and are always at end of url, they can run about 400 more raffles before we need to change this
      var craftedRaffleUrl = $"https://raffles-resources.jdsports.co.uk/raffles/raffles_{raffleId}.js";

      var regexPattern = @"""size_options"":.*},""";
      var rg = new Regex(regexPattern);

      var getJsonArray = await HttpClient.GetAsync(craftedRaffleUrl, ct);
      var jsonArrayString = await getJsonArray.Content.ReadAsStringAsync();
      var findSizeOptions = rg.Match(jsonArrayString).ToString();
      var
        jObject = findSizeOptions.Remove(findSizeOptions.Length - 2)
          .Replace(@"""size_options"":",
            ""); //the "sizeOptions" ends with " ," ", so to not cause newtonsoft errors, last char is deleted, along with "size_options"

      var sizeDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jObject);

      var sizeValue =
        sizeDictionary.FirstOrDefault(x => x.Value.Contains(size))
          .Key; //find value containing the size, then lookup the key
      return sizeValue;
    }


    public async Task<bool> SubmitAsync(FootpatrolSubmitPayload payload, CancellationToken ct)
    {
      var raffleId = payload.RaffleUrl.Substring(payload.RaffleUrl.Length - 3);

      var day = _birthdayProviderService.GetDate();
      var month = _birthdayProviderService.GetMonth();
      var year = _birthdayProviderService.GetYear();
      var dob = $"{day}/{month}/{year}";

      var raffleRoot = new
      {
        address1 = payload.Profile.ShippingAddress.AddressLine1,
        address2 = payload.Profile.ShippingAddress.AddressLine2,
        city = payload.Profile.ShippingAddress.City,
        county = payload.County,
        dateofBirth = dob,
        email = payload.Email,
        paypalEmail = payload.PaypalEmail,
        email_optin = 0,
        firstName = payload.Profile.ShippingAddress.FirstName,
        hostname = "https://raffles.footpatrol.com",
        lastName = payload.Profile.ShippingAddress.LastName,
        mobile = payload.Profile.ShippingAddress.PhoneNumber,
        postCode = payload.Profile.ShippingAddress.ZipCode,
        rafflesID = raffleId,
        shoeSize = payload.SizeValue,
        siteCode = "FPUK",
        sms_optin = 0,
        token = payload.Captcha
      };
      var raffleSerialize = JsonConvert.SerializeObject(raffleRoot);
      var raffleData = new StringContent(raffleSerialize, Encoding.UTF8, "application/json");
      var posturl = "https://nk7vfpucy5.execute-api.eu-west-1.amazonaws.com/prod/save_entry";
      var response = await HttpClient.PostAsync(posturl, raffleData, ct);

      return response.IsSuccessStatusCode;
    }
  }
}