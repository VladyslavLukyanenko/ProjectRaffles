using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketNewYorkModule;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketNewYorkInstoreModule
{
  public class DoverStreetMarketNewYorkInstoreClient : ModuleHttpClientBase, IDoverStreetMarketNewYorkInstoreClient
  { 
      private readonly IStringUtils _stringUtils;

    public DoverStreetMarketNewYorkInstoreClient(IStringUtils stringUtils)
    {
      _stringUtils = stringUtils;
    }

    protected override void ConfigureHttpClient(HttpClientOptions options)
    {
      options.PostConfigure = httpClient =>
      {
        httpClient.DefaultRequestHeaders.Add("User-Agent",
          "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
        httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        httpClient.DefaultRequestHeaders.Add("dnt", "1");
        httpClient.DefaultRequestHeaders.Add("Accept-Language","en-US");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","document");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","navigate");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","cross-site");
        httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
      };
    }

    public async Task<DoverStreetMarketNewYorkInstoreParsed> ParseRaffleAsync(string raffleurl, string question, CancellationToken ct)
    {
      if (!raffleurl.StartsWith("http"))
      {
        raffleurl = "https://" + raffleurl;
      }

      Uri uri = new Uri(raffleurl);
      var baseurl = uri.Host;

      //the following will find the DSM formstack link and GET it
      var pattern = @"src=""https:\/\/doverstreetmarketinternational\.formstack\.com\/forms\/js\.php\/.*""><";
      Regex rg = new Regex(pattern);
      var dsmBase = await HttpClient.GetAsync(raffleurl, ct);
      if(!dsmBase.IsSuccessStatusCode) await dsmBase.FailWithRootCauseAsync("Can't access site", ct);
      
      var dsmContentBase = await dsmBase.Content.ReadAsStringAsync(ct);
      var findFormstackSite = rg.Match(dsmContentBase);
      var formstackSite = findFormstackSite.ToString().Replace(@"src=""", "").Replace(@"""><", "").Replace(@"/js.php","");
      
      var getFormstackSite = await HttpClient.GetAsync(formstackSite, ct);
      if(!getFormstackSite.IsSuccessStatusCode) await getFormstackSite.FailWithRootCauseAsync("Can't access formstack site", ct);
      var formstackHtml = await getFormstackSite.Content.ReadAsStringAsync(ct);

      //here we will parse the formstack html
      var doc = new HtmlDocument();
      doc.LoadHtml(formstackHtml);

      //find form details
      var form = doc.DocumentNode.SelectSingleNode("//input[@name='form']").GetAttributeValue("value", "");
      var viewkey = doc.DocumentNode.SelectSingleNode("//input[@name='viewkey']").GetAttributeValue("value", "");
      var viewparam = doc.DocumentNode.SelectSingleNode("//input[@name='viewparam']").GetAttributeValue("value", "");

      //find form fields
      var fullnameField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Full Name']")
        .GetAttributeValue("id", "").Replace("fsCell", "field");
      var phonenumberField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Mobile Phone Number']")
        .GetAttributeValue("id", "").Replace("fsCell", "field");
      var emailField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Email']")
        .GetAttributeValue("id", "").Replace("fsCell", "field");


      var addressField = doc.DocumentNode
        .SelectSingleNode("//div[@fs-field-validation-name='First Line of Billing Address']")
        .GetAttributeValue("id", "").Replace("fsCell", "field");

      var zipCodeField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Zip Code']")
        .GetAttributeValue("id", "").Replace("fsCell", "field");

      var mailingListField =  doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Mailing List']").GetAttributeValue("id", "").Replace("fsCell","field"); //needed on DSMNY
      var mailingList = mailingListField + "[]";

      var sizeRegexPattern = @"id="".*"" lang=""en"" fs-field-type=""select"" fs-field-validation-name="".*Size";
      var sizeRegex = new Regex(sizeRegexPattern);
      var findSizeField = sizeRegex.Match(formstackHtml).ToString();
        
      var removeSizeRegexPattern = @""" lang=""en"" fs-field-type=""select"" fs-field-validation-name="".*Size";
      var removeSizeRegex = new Regex(removeSizeRegexPattern);
      var removeSizeMatch = removeSizeRegex.Match(findSizeField).ToString();
        
      var finalSize = findSizeField.Replace(@"id=""", "")
          .Replace(removeSizeMatch,"")
          .Replace("fsCell", "field");
      
      var questionField = "";
      if (question != null)
      {
        questionField = doc.DocumentNode.SelectSingleNode($"//div[@fs-field-validation-name='{question}']")
          .GetAttributeValue("id", "").Replace("fsCell", "field");
      }

      return new DoverStreetMarketNewYorkInstoreParsed(form, viewkey, viewparam, fullnameField, emailField, phonenumberField, finalSize,  addressField, zipCodeField, mailingList, questionField);
    }

    public async Task<bool> SubmitAsync(DoverStreetMarketNewYorkInstorePayload payload, CancellationToken ct)
    {
      var webkitBoundary = await _stringUtils.GenerateRandomStringAsync(16);
      var nonce = await _stringUtils.GenerateRandomStringAsync(16);
      
      var content = new MultipartFormDataContent("----WebKitFormBoundary" + webkitBoundary)
      {
        {new StringContent(payload.Parsed.Form), "form"},
        {new StringContent(payload.Parsed.Viewkey), "viewkey"},
        {new StringContent(""), "password"},
        {new StringContent(""), "hidden_fields"},
        {new StringContent(""), "incomplete"},
        {new StringContent(""), "incomplete_password"},
        {new StringContent("https://newyork.doverstreetmarket.com/"), "referrer"},
        {new StringContent("js"), "referrer_type"},
        {new StringContent("1"), "_submit"},
        {new StringContent("3"), "style_version"},
        {new StringContent(payload.Parsed.Viewparam), "viewparam"},
        {new StringContent(payload.Address.FullName.Value), payload.Parsed.FullnameField},
        {new StringContent(payload.Email), payload.Parsed.EmailField},
        {new StringContent(payload.Address.PhoneNumber.Value), payload.Parsed.PhoneNumberField},
        {new StringContent(payload.Size), payload.Parsed.SizeField},
        {new StringContent(payload.Address.PostCode.Value), payload.Parsed.ZipCodeField},
        {new StringContent(payload.Address.AddressLine1.Value), payload.Parsed.AddressField},
      };
      
      //done like this to make sure order is correct
      if (payload.UseMailingList.Equals("Y"))
      {
        var mailingList = new StringContent("I do not want to be added to the DSMNY mailing list");
        content.Add(mailingList, payload.Parsed.MailingList);
      }
      
      if (payload.QuestionAnswer != null)
      {
        var questionAnswer = new StringContent(payload.QuestionAnswer);
        content.Add(questionAnswer, payload.Parsed.QuestionField);
      }

      var captcha = new StringContent(payload.Captcha);
      var noncePayload = new StringContent(nonce);
      content.Add(captcha, "g-recaptcha-response");
      content.Add(noncePayload,"nonce");

      HttpClient.DefaultRequestHeaders.Add("origin","https://newyork.doverstreetmarket.com");
      HttpClient.DefaultRequestHeaders.Add("referer","https://newyork.doverstreetmarket.com/");
      var endpoint = "https://doverstreetmarketinternational.formstack.com/forms/index.php";
      var response = await HttpClient.PostAsync(endpoint, content, ct);
      
      if (!response.IsSuccessStatusCode)
      {
        var responseBody = await response.Content.ReadAsStringAsync(ct);
        if(responseBody.Contains("security")) await response.FailWithRootCauseAsync("Captcha error", ct);
        if(responseBody.Contains("429")) await response.FailWithRootCauseAsync("Proxy error", ct);
        await response.FailWithRootCauseAsync("Error on submission", ct);
      }
      
      return response.IsSuccessStatusCode;
    }
  }
}