using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.JDSportsModule
{
  public class JDSportsClient : ModuleHttpClientBase, IJDSportsClient
  {
    private readonly IBirthdayProviderService _birthdayProviderService;
    private readonly ICaptchaSolveService _captchaSolver;

    public JDSportsClient(IBirthdayProviderService birthdayProviderService, ICaptchaSolveService captchaSolver)
    {
      _birthdayProviderService = birthdayProviderService;
      _captchaSolver = captchaSolver;
    }

    public Task<string> GetBaseUrlAsync(string raffleurl)
    {
      var uri = new Uri(raffleurl);
      var baseUrl = uri.Host;
      return Task.FromResult(baseUrl);
    }

    public Task<string> GetStoreCountryAsync(string baseurl)
    {
      var result = baseurl switch
      {
        "raffles.jdsports.es" => "JDES",
        "raffles.jdsports.co.uk" => "JDUK",
        "raffles.jdsports.it" => "JDIT",
        "raffles.jdsports.de" => "JDDE",
        "raffles.jdsports.se" => "JDSE",
        "raffles.jdsports.fr" => "JDFR",
        "raffles.jd-sports.com.au" => "JDAU",
        _ => throw new ArgumentOutOfRangeException(baseurl, "URL not supported")
      };

      return Task.FromResult(result);
    }

    public async Task<string> SolveCaptchaAsync(string raffleurl, string baseurl, CancellationToken ct)
    {
      var siteKey = await Sitekeys(baseurl);
      var captcha = await _captchaSolver.SolveReCaptchaV2Async(siteKey, raffleurl, true, ct);

      return captcha;
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
      if(!getJsonArray.IsSuccessStatusCode) throw new InvalidOperationException("Can't access the size selection");
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


    public async Task<bool> SubmitAsync(JDSportsSubmitPayload payload, CancellationToken ct)
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
        paypalEmail = payload.Paypalemail,
        email_optin = 1,
        firstName = payload.Profile.ShippingAddress.FirstName,
        hostname = payload.BaseUrl,
        lastName = payload.Profile.ShippingAddress.LastName,
        mobile = payload.Profile.ShippingAddress.PhoneNumber,
        postCode = payload.Profile.ShippingAddress.ZipCode,
        rafflesID = raffleId,
        shoeSize = payload.SizeValue,
        siteCode = payload.Store,
        sms_optin = 0,
        token = payload.Captcha
      };
      var raffleSerialize = JsonConvert.SerializeObject(raffleRoot);
      var raffleData = new StringContent(raffleSerialize, Encoding.UTF8, "application/json");


      var posturl =
        "https://nk7vfpucy5.execute-api.eu-west-1.amazonaws.com/prod/save_entry"; //todo: setup parser to automatically find posturl
      var response = await HttpClient.PostAsync(posturl, raffleData, ct);

      return response.IsSuccessStatusCode;
    }

    private Task<string> Sitekeys(string baseUrl)
    {
      var result = baseUrl switch
      {
        "raffles.jdsports.es" => "6LczUsQUAAAAAMGiG9ai0CK_g2HAadjIKRmyAc8",
        "raffles.jdsports.co.uk" => "6LdTCMAUAAAAAP_h-Uqc7LqpUnZKYeFpowq29qQj",
        "raffles.jdsports.it" => "6LdRUsQUAAAAAN-HEzo2_L0n0m-mLFzvIk7gYZaf",
        "raffles.jdsports.de" => "6LcdUsQUAAAAAMKoEoK_Y9cwXZx-CaTh8KdipUBV",
        "raffles.jdsports.se" => "6LdWUsQUAAAAAB0M0OOP5QoNTXUUjvQ61ciPuIwE",
        "raffles.jdsports.fr" => "6LfIAsAUAAAAAFV9yLvpbaOMUCKyU0ATSk56vKZM",
        "raffles.jd-sports.com.au" => "6LeYhvkUAAAAAJUThPTMUPJpkkxhw3teNJXBijaU",
        _ => throw new ArgumentOutOfRangeException(baseUrl, "URL not supported")
      };

      return Task.FromResult(result);
    }
  }
}