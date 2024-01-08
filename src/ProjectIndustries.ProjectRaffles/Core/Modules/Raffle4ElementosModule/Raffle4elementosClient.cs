using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.Raffle4ElementosModule
{
  public class Raffle4elementosClient : ModuleHttpClientBase, IRaffle4elementosClient
  {
    private readonly ICountriesService _countriesService;
    private readonly IBirthdayProviderService _birthdayService;

    public Raffle4elementosClient(ICountriesService countriesService, IBirthdayProviderService birthdayService)
    {
      _countriesService = countriesService;
      _birthdayService = birthdayService;
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

    public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
    {
      var getPage = await HttpClient.GetAsync(raffleurl, ct);
      var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(body);

      var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

      return node;
    }

    public async Task<Raffle4elementosParsed> ParseRaffleAsync(string raffleurl, CancellationToken ct)
    {
    /*  var getBody = await HttpClient.GetAsync(raffleurl, ct);
      var body = await getBody.ReadStringResultOrFailAsync("Can't access site", ct);
      
      var regexPattern = @"src=https:\/\/app3\.salesmanago\.pl\/ms\/.*\.htm";
      var regex = new Regex(regexPattern);
      var salesSiteMatch = regex.Match(body).ToString();
      var salesSite = salesSiteMatch.Replace("src=", ""); */
      
      var getSalesSite = await HttpClient.GetAsync(raffleurl, ct);
      var salesBody = await getSalesSite.ReadStringResultOrFailAsync("Can't get form", ct);

      var doc = new HtmlDocument();
      doc.LoadHtml(salesBody);

      var form = "";
      var formid = "";
      try
      { 
        form = doc.DocumentNode.SelectSingleNode("//form").GetAttributeValue("action", "");
        formid = doc.DocumentNode.SelectSingleNode("//input[@name='formId']").GetAttributeValue("value", "");
      }
      catch (NullReferenceException e)
      {
        throw new RaffleFailedException(message: "Error under parsing", rootCause: $"{e}, HTML: {salesBody}");
      }

      return new Raffle4elementosParsed(form, formid, raffleurl);
      
    }


    public async Task<bool> SubmitAsync(Raffle4elementosSubmitPayload payload, CancellationToken ct)
    {
      var country = _countriesService.GetCountryName(payload.Profile.CountryId.Value)
        .Replace(" (" + payload.Profile.CountryId.Value + ")", "");
      
      var fullname = payload.Profile.FirstName.Value + " " + payload.Profile.LastName.Value;
      var birthDay = await _birthdayService.GetDate();
      var birthMonth = await _birthdayService.GetMonth();
      var birthYear = await _birthdayService.GetYear();
      var dateOfBirth = $"{birthYear}/{birthMonth}/{birthDay}";


      //craft url
      var pos = payload.ParsedRaffle.Endpoint.IndexOf("/ms", StringComparison.Ordinal);
      var afterbase = payload.ParsedRaffle.Endpoint.Remove(pos);

      var craftedUrl = afterbase + payload.ParsedRaffle.Form + "?lang=en" + $"&formId={payload.ParsedRaffle.FormId}" +
                       $"&sm-form-email={payload.Email}" +
                       $"&sm-form-name={fullname}" + $"&sm-form-birthday={dateOfBirth}" +
                       $"&sm-form-street={payload.Profile.AddressLine1.Value}" +
                       $"&sm-form-city={payload.Profile.City.Value}" +
                       $"&sm-form-province={payload.Profile.ProvinceId.Value}" +
                       $"&sm-form-zip={payload.Profile.PostCode.Value}" +
                       $"&sm-form-country={country}" +
                       $"&sm-form-phone={payload.Profile.PhoneNumber.Value}" +
                       $"&sm-cst.instagram_user={payload.InstaHandle}" +
                       $"&sm-cst.size={payload.SizeValue}" + "&sm-form-consent-id-1601-POLITICAPRIVACIDAD=true" +
                       "&sm-form-consent-name-POLITICAPRIVACIDAD=true" + "&sm-form-agreement_agreement_2=true";
      //todo: check if  "sm-form-consent-id-1601" ever changes id value

      var replacedUrl = craftedUrl.UriEscape();

      var raffleresponse = await HttpClient.GetAsync(replacedUrl, ct);
      var raffle = await raffleresponse.ReadStringResultOrFailAsync("error on submission", ct);

      if (!raffle.Contains("Check your email")) await raffleresponse.FailWithRootCauseAsync("Error on submission", ct);

      return raffle.Contains("Check your email");
    }
  }
}