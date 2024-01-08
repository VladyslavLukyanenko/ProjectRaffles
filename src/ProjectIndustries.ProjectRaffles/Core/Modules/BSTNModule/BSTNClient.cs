using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.BSTNModule
{
  public class BSTNClient : ModuleHttpClientBase, IBSTNClient
  {
    private readonly ICountriesService _countriesService;
    private readonly IBirthdayProviderService _birthdayProvider;

    public BSTNClient(ICountriesService countriesService, IBirthdayProviderService birthdayProvider)
    {
      _birthdayProvider = birthdayProvider;
      _countriesService = countriesService;
    }

    public async Task<bool> SubmitAsync(BSTNSubmitPayload payload, CancellationToken ct)
    {
      var raffleID = payload.RaffleUrl.Replace("https://raffle.bstn.com/", "");

      var countryName = _countriesService.GetCountryName(payload.Profile.ShippingAddress.CountryId)
        .Replace(" (" + payload.Profile.ShippingAddress.CountryId + ")", "");

      var birthDay = await _birthdayProvider.GetDate();
      var birthMonth = await _birthdayProvider.GetMonth();

      var bstnRoot = new
      {
        acceptedPrivacy = true,
        additional = new
        {
          instagram = payload.InstaHandle
        },
        address2 = payload.Profile.ShippingAddress.AddressLine2,
        bDay = birthDay,
        bMonth = birthMonth, 
        city = payload.Profile.ShippingAddress.City,
        country = countryName,
        email = payload.Email,
        firstName = payload.Profile.ShippingAddress.FirstName,
        lastName = payload.Profile.ShippingAddress.LastName,
        newsletter = false,
        raffle = new
        {
          option = payload.SizeValue, //formatted like "45 1/3"
          parentIndex = 0,
          raffleId = raffleID
        },
        recaptchaToken = payload.Captcha,
        street = payload.Profile.ShippingAddress.AddressLine1,
        streetno = "10", //todo: split up addressline1 so StreetNo is separate
        title = "Mr.",
        zip = payload.Profile.ShippingAddress.ZipCode
      };

      var customer = JsonConvert.SerializeObject(bstnRoot);
      var content = new StringContent(customer, Encoding.UTF8, "application/json");


      //todo: get cloudflare cookies, they need to be submitted with the request

      var signupUrl = "https://raffle.bstn.com/api/register";

      var raffleresponse = await HttpClient.PostAsync(signupUrl, content, ct);

      return raffleresponse.IsSuccessStatusCode;
    }
  }
}