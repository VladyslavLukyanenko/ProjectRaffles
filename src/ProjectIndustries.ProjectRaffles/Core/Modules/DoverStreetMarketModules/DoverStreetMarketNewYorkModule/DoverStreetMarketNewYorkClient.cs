using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using DiscordRPC.Helper;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketNewYorkModule
{
  public class DoverStreetMarketNewYorkClient : ModuleHttpClientBase, IDoverStreetMarketNewYorkClient
  {
    private readonly ICountriesService _countriesService;
    private readonly IStringUtils _stringUtils;

    public DoverStreetMarketNewYorkClient(ICountriesService countriesService, IStringUtils stringUtils)
    {
      _countriesService = countriesService;
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

    public async Task<DoverStreetMarketNewYorkParsedRaffleFields> ParseRaffleAsync(string raffleurl, string variant, string parseType, string question, CancellationToken ct)
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
      
      var stateField = "NOTUSED";
      try //sometimes they forget to include state field lmao
      {
        stateField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='State']")
          .GetAttributeValue("id", "").Replace("fsCell", "field");
      }
      catch (Exception e)
      {
        stateField = "NOTUSED";
      }

      var zipCodeField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Zip Code']")
        .GetAttributeValue("id", "").Replace("fsCell", "field");

      var mailingListField =  doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Mailing List']").GetAttributeValue("id", "").Replace("fsCell","field"); //needed on DSMNY
      var mailingList = mailingListField + "[]";
      
       //variant field
      var colourField = "";
      var hiddenFields = "";
      var finalSize = "";

      if (variant != null)
      {
        
        //identify if style or colour field
        var findColour = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Colour']");
        if (findColour != null)
        {
          colourField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Colour']")
            .GetAttributeValue("id", "").Replace("fsCell", "field");
        }
        var findColor = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Color']"); //american spelling
        if (findColor != null)
        {
          colourField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Color']")
              .GetAttributeValue("id", "").Replace("fsCell", "field");
        }
        var findStyle = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Style']");
        if (findStyle != null)
        {
          colourField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Style']")
            .GetAttributeValue("id", "").Replace("fsCell", "field");
        }
        
        if (parseType == "NY") //simple size parsing with no hiddenFields matching
        {
          var sizeRegexPattern = @"id="".*"" lang=""en"" fs-field-type=""select"" fs-field-validation-name=""Size";
          var sizeRegex = new Regex(sizeRegexPattern);
          var findSizeField = sizeRegex.Match(formstackHtml).ToString();

          var removeSizeRegexPattern = @""" lang=""en"" fs-field-type=""select"" fs-field-validation-name="".*Size";
          var removeSizeRegex = new Regex(removeSizeRegexPattern);
          var removeSizeMatch = removeSizeRegex.Match(findSizeField).ToString();

          finalSize = findSizeField.Replace(@"id=""", "")
            .Replace(removeSizeMatch, "")
            .Replace("fsCell", "field");
        }

        if (parseType == "LDN") //same we use in DSMLDN which also matches hiddenfields
        {
          //find all sizes and add them to a list
          var allSizesRegexPattern =
            @"target: '\d*', action: 'Show', bool: 'OR', fields: \['\d*'\],checks: \[{field: '\d*', condition: '==', option: '.*'";
          var allSizesRegex = new Regex(allSizesRegexPattern);

          var matches = allSizesRegex.Matches(formstackHtml);
          var hiddenFieldsList = new List<string>();
          foreach (var match in matches)
          {
            var allSizesMatch = match.ToString();

            var removeAllSizesRegexPattern =
              @"', action: 'Show', bool: 'OR', fields: \['\d*'\],checks: \[{field: '\d*', condition: '==', option: '.*'";
            var removeAllSizesRegex = new Regex(removeAllSizesRegexPattern);
            var removeAllSizesMatch = removeAllSizesRegex.Match(allSizesMatch).ToString();

            var fieldMatch = allSizesMatch.Replace(removeAllSizesMatch, "").Replace("target: '", "");
            var finalMatch = "field" + fieldMatch;
            hiddenFieldsList.Add(finalMatch);
          }
          
          if (variant.Contains("'")) variant = variant.Replace("'", @"\\'");
          if (variant.Contains("(") || variant.Contains(")")) variant = variant.Replace("(", @"\(").Replace(")", @"\)");


          //find variant specified size field
          var sizeRegexPattern =
            @"target: '\d*', action: 'Show', bool: 'OR', fields: \['\d*'\],checks: \[{field: '\d*', condition: '==', option: '" +
            variant.Replace("/", @"\/") + "'";
          var sizeRegex = new Regex(sizeRegexPattern);
          var sizeMatch = sizeRegex.Match(formstackHtml).ToString();

          var removeSizeRegexPattern =
            @"', action: 'Show', bool: 'OR', fields: \['\d*'\],checks: \[{field: '\d*', condition: '==', option: '" +
            variant.Replace("/", @"\/") + "'";
          var removeSizeRegex = new Regex(removeSizeRegexPattern);
          var removeSizeMatch = removeSizeRegex.Match(sizeMatch).ToString();
          finalSize = "field" + sizeMatch.Replace(removeSizeMatch, "").Replace("target: '", "");

          //remove variant specified sizeField
          hiddenFieldsList.Remove(finalSize);
          hiddenFields = string.Join(",", hiddenFieldsList);
        }
      }
      else
      {
        var sizeRegexPattern = @"id="".*"" lang=""en"" fs-field-type=""select"" fs-field-validation-name=""Size";
        var sizeRegex = new Regex(sizeRegexPattern);
        var findSizeField = sizeRegex.Match(formstackHtml).ToString();
        
        var removeSizeRegexPattern = @""" lang=""en"" fs-field-type=""select"" fs-field-validation-name="".*Size";
        var removeSizeRegex = new Regex(removeSizeRegexPattern);
        var removeSizeMatch = removeSizeRegex.Match(findSizeField).ToString();
        
        finalSize = findSizeField.Replace(@"id=""", "")
          .Replace(removeSizeMatch,"")
          .Replace("fsCell", "field");
      }
      
      var questionField = "";
      if (question != null)
      {
        questionField = doc.DocumentNode.SelectSingleNode($"//div[@fs-field-validation-name='{question}']")
          .GetAttributeValue("id", "").Replace("fsCell", "field");
      }

      return new DoverStreetMarketNewYorkParsedRaffleFields(form, viewkey, viewparam, fullnameField, phonenumberField,
        emailField, addressField, finalSize, baseurl, stateField, zipCodeField, formstackSite, mailingList, colourField, hiddenFields, questionField);
    }

    public async Task<bool> SubmitAsync(DoverStreetMarketNewYorkSubmitPayload payload, CancellationToken ct)
    {
      var content = await CreateContent(payload);

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

    private async Task<MultipartFormDataContent> CreateContent(DoverStreetMarketNewYorkSubmitPayload payload)
    {
      if (payload.Variant != null)
      {
        var contentWithVariant = await CreateContentWithVariant(payload);
        return contentWithVariant;
      }

      var contentWithoutVariant = await CreateContentWithoutVariant(payload);
      return contentWithoutVariant;
    }

    private async Task<MultipartFormDataContent> CreateContentWithVariant(DoverStreetMarketNewYorkSubmitPayload payload)
    {
      var webkitBoundary = await _stringUtils.GenerateRandomStringAsync(16);

      var state = _countriesService.GetProvinceName(payload.Profile.CountryId,
        payload.Profile.ProvinceId);
      
      if(string.IsNullOrEmpty(state)) throw new RaffleFailedException(message: "State is not selected", rootCause: "State null");
 
      var content = new MultipartFormDataContent("----WebKitFormBoundary" + webkitBoundary)
      {
        {new StringContent(payload.ParsedRaffle.Form), "form"},
        {new StringContent(payload.ParsedRaffle.Viewkey), "viewkey"},
        {new StringContent(""), "password"},
        {new StringContent(payload.ParsedRaffle.HiddenFields), "hidden_fields"},
        {new StringContent(""), "incomplete"},
        {new StringContent(""), "incomplete_password"},
        {new StringContent(payload.ParsedRaffle.BaseUrl), "referrer"},
        {new StringContent("js"), "referrer_type"},
        {new StringContent("1"), "_submit"},
        {new StringContent("3"), "style_version"},
        {new StringContent(payload.ParsedRaffle.Viewparam), "viewparam"},
        {new StringContent(payload.Profile.FullName), payload.ParsedRaffle.FullnameField},
        {new StringContent(payload.Email), payload.ParsedRaffle.EmailField},
        {new StringContent(payload.Profile.PhoneNumber), payload.ParsedRaffle.PhoneNumberField},
        {new StringContent(payload.Variant), payload.ParsedRaffle.ColourField},
        {new StringContent(payload.SizeValue), payload.ParsedRaffle.SizeField},
        {new StringContent(payload.Profile.PostCode), payload.ParsedRaffle.ZipCodeField},
        {new StringContent(payload.Profile.AddressLine1), payload.ParsedRaffle.AddressField},
      };
      

      if (payload.ParsedRaffle.StateField != "NOTUSED")
      {
        var stateContent = new StringContent(state);
        content.Add(stateContent, payload.ParsedRaffle.StateField); //sometimes they forget to include state field lmao
      }
      
      
      //done like this to make sure order is correct
      if (payload.UseMailingList.Equals("Y"))
      {
        var mailingList = new StringContent("I do not want to be added to the DSMNY mailing list");
        content.Add(mailingList, payload.ParsedRaffle.MailingList);
      }
      
      if (payload.QuestionAnswer != null)
      {
        var questionAnswer = new StringContent(payload.QuestionAnswer);
        content.Add(questionAnswer, payload.ParsedRaffle.QuestionField);
      }

      var captcha = new StringContent(payload.Captcha);
      var nonce = new StringContent(payload.Nonce);
      content.Add(captcha, "g-recaptcha-response");
      content.Add(nonce,"nonce");

      return content;
    }

    private async Task<MultipartFormDataContent> CreateContentWithoutVariant(DoverStreetMarketNewYorkSubmitPayload payload)
    {
      var webkitBoundary = await _stringUtils.GenerateRandomStringAsync(16);

      var state = _countriesService.GetProvinceName(payload.Profile.CountryId,
        payload.Profile.ProvinceId);

      if(string.IsNullOrEmpty(state)) throw new RaffleFailedException(message: "State is not selected", rootCause: "State null");
      
      
      var content = new MultipartFormDataContent("----WebKitFormBoundary" + webkitBoundary)
      {
        {new StringContent(payload.ParsedRaffle.Form), "form"},
        {new StringContent(payload.ParsedRaffle.Viewkey), "viewkey"},
        {new StringContent(""), "password"},
        {new StringContent(""), "hidden_fields"},
        {new StringContent(""), "incomplete"},
        {new StringContent(""), "incomplete_password"},
        {new StringContent("https://"+payload.ParsedRaffle.BaseUrl), "referrer"},
        {new StringContent("js"), "referrer_type"},
        {new StringContent("1"), "_submit"},
        {new StringContent("3"), "style_version"},
        {new StringContent(payload.ParsedRaffle.Viewparam), "viewparam"},
        {new StringContent(payload.Profile.FullName), payload.ParsedRaffle.FullnameField},
        {new StringContent(payload.Email), payload.ParsedRaffle.EmailField},
        {new StringContent(payload.Profile.PhoneNumber), payload.ParsedRaffle.PhoneNumberField},
        {new StringContent(payload.SizeValue), payload.ParsedRaffle.SizeField},
        {new StringContent(payload.Profile.PostCode), payload.ParsedRaffle.ZipCodeField},
        {new StringContent(payload.Profile.AddressLine1), payload.ParsedRaffle.AddressField},
        //{new StringContent(state), payload.ParsedRaffle.StateField}, //state field
      };
      
      if (payload.ParsedRaffle.StateField != "NOTUSED")
      {
        var stateContent = new StringContent(state);
        content.Add(stateContent, payload.ParsedRaffle.StateField); //sometimes they forget to include state field lmao
      }
      
      if (payload.UseMailingList.Equals("Y"))
      {
        var mailingList = new StringContent("I do not want to be added to the DSMNY mailing list");
        content.Add(mailingList, payload.ParsedRaffle.MailingList);
      }
      
      if (payload.QuestionAnswer != null)
      {
        var questionAnswer = new StringContent(payload.QuestionAnswer);
        content.Add(questionAnswer, payload.ParsedRaffle.QuestionField);
      }

      var captcha = new StringContent(payload.Captcha);
      var nonce = new StringContent(payload.Nonce);
      content.Add(captcha, "g-recaptcha-response");
      content.Add(nonce,"nonce");

      return content;
    }
  }
}