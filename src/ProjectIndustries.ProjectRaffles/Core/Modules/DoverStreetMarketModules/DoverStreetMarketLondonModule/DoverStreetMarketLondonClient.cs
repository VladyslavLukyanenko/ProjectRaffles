using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DoverStreetMarketLondonModule
{
  public class DoverStreetMarketLondonClient : ModuleHttpClientBase, IDoverStreetMarketLondonClient
  {
    private readonly ICountriesService _countriesService;
    private readonly IStringUtils _stringUtils;

    public DoverStreetMarketLondonClient(ICountriesService countriesService, IStringUtils stringUtils)
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
        httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","document");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","navigate");
        httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","cross-site");
        httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
      };
    }

    public async Task<DoverStreetMarketLondonParsedRaffleFields> ParseRaffleAsync(string raffleurl, string variant, string question, bool containsHiddenFields, CancellationToken ct)
    {
      if (!raffleurl.StartsWith("http"))
      {
        raffleurl = "https://" + raffleurl;
      }

      var uri = new Uri(raffleurl);
      var baseurl = uri.Host;

      //the following will find the DSM formstack link and GET it
      var pattern = @"src=""https:\/\/doverstreetmarketinternational\.formstack\.com\/forms\/js\.php\/.*""><";
      var rg = new Regex(pattern);
      var dsmBase = await HttpClient.GetAsync(raffleurl, ct);
      var dsmContentBase = await dsmBase.ReadStringResultOrFailAsync("Can't access site", ct);
      
      var findFormstackSite = rg.Match(dsmContentBase);
      var formstackSite = findFormstackSite.ToString().Replace(@"src=""", "").Replace(@"""><", "").Replace(@"/js.php","");
      
      var getFormstackSite = await HttpClient.GetAsync(formstackSite, ct);
      while (!getFormstackSite.IsSuccessStatusCode)
      {
        findFormstackSite = findFormstackSite.NextMatch();
        formstackSite = findFormstackSite.ToString().Replace(@"src=""", "").Replace(@"""><", "").Replace(@"/js.php","");
        getFormstackSite = await HttpClient.GetAsync(formstackSite, ct);
      }
      var formstackHtml = await getFormstackSite.ReadStringResultOrFailAsync("Can't access form", ct);

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
      
      var countryField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Country']")
        .GetAttributeValue("id", "").Replace("fsCell", "field");
      var postcodeField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Post Code']")
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
        var findStyle = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Style']");
        if (findStyle != null)
        {
          colourField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Style']")
            .GetAttributeValue("id", "").Replace("fsCell", "field");
        }

        if (containsHiddenFields)
        {
          //find all sizes and add them to a list
        var allSizesRegexPattern = @"target: '\d*', action: 'Show', bool: 'OR', fields: \['\d*'\],checks: \[{field: '\d*', condition: '==', option: '.*'";
        var allSizesRegex = new Regex(allSizesRegexPattern);

        var matches = allSizesRegex.Matches(formstackHtml);
        var hiddenFieldsList = new List<string>();
        foreach(var match in matches)
        {
          var allSizesMatch = match.ToString();
        
          var removeAllSizesRegexPattern = @"', action: 'Show', bool: 'OR', fields: \['\d*'\],checks: \[{field: '\d*', condition: '==', option: '.*'";
          var removeAllSizesRegex = new Regex(removeAllSizesRegexPattern);
          var removeAllSizesMatch = removeAllSizesRegex.Match(allSizesMatch).ToString();
        
          var fieldMatch = allSizesMatch.Replace(removeAllSizesMatch, "").Replace("target: '","");
          var finalMatch = "field" + fieldMatch;
          hiddenFieldsList.Add(finalMatch);
        }

        if (variant.Contains("'")) variant = variant.Replace("'", @"\\'");
        if (variant.Contains("(") || variant.Contains(")")) variant = variant.Replace("(", @"\(").Replace(")", @"\)");
        
        //find variant specified size field
        var sizeRegexPattern = @"target: '\d*', action: 'Show', bool: 'OR', fields: \['\d*'\],checks: \[{field: '\d*', condition: '==', option: '" + variant.Replace("/",@"\/") + "'";
        var sizeRegex = new Regex(sizeRegexPattern);
        var sizeMatch = sizeRegex.Match(formstackHtml).ToString();
      
        var removeSizeRegexPattern = @"', action: 'Show', bool: 'OR', fields: \['\d*'\],checks: \[{field: '\d*', condition: '==', option: '" + variant.Replace("/",@"\/") + "'";
        var removeSizeRegex = new Regex(removeSizeRegexPattern);
        var removeSizeMatch = removeSizeRegex.Match(sizeMatch).ToString();
        finalSize = "field" + sizeMatch.Replace(removeSizeMatch, "").Replace("target: '", "");
      
        //remove variant specified sizeField
        hiddenFieldsList.Remove(finalSize);
        hiddenFields = string.Join(",", hiddenFieldsList);
        } 
        else
        {
          var sizeRegexPattern = @"id="".*"" lang=""en"" fs-field-type=""select"" fs-field-validation-name="".*Size";
          var sizeRegex = new Regex(sizeRegexPattern);
          var findSizeField = sizeRegex.Match(formstackHtml).ToString();
        
          var removeSizeRegexPattern = @""" lang=""en"" fs-field-type=""select"" fs-field-validation-name="".*Size";
          var removeSizeRegex = new Regex(removeSizeRegexPattern);
          var removeSizeMatch = removeSizeRegex.Match(findSizeField).ToString();
        
          finalSize = findSizeField.Replace(@"id=""", "")
            .Replace(removeSizeMatch,"")
            .Replace("fsCell", "field");
        }
      }
      else
      {
        var sizeRegexPattern = @"id="".*"" lang=""en"" fs-field-type=""select"" fs-field-validation-name="".*Size";
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

      var shippingField = "";

      try
      {
        shippingField = doc.DocumentNode.SelectSingleNode("//div[@fs-field-validation-name='Shipping / Collection']")
          .GetAttributeValue("id", "").Replace("fsCell", "field");
      }
      catch (Exception e)
      {
        shippingField = "NOTUSED";
      }

      return new DoverStreetMarketLondonParsedRaffleFields(form, viewkey, viewparam, fullnameField, phonenumberField,
        emailField, addressField, finalSize, baseurl, countryField, postcodeField, formstackSite, mailingList, colourField, hiddenFields, questionField, shippingField);
    }
    


    public async Task<bool> SubmitAsync(DoverStreetMarketLondonSubmitPayload payload, CancellationToken ct)
    {
      var content = await CreateContent(payload, ct);

      HttpClient.DefaultRequestHeaders.Add("origin","https://london.doverstreetmarket.com");
      HttpClient.DefaultRequestHeaders.Add("referer","https://london.doverstreetmarket.com/");
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

    private async Task<MultipartFormDataContent> CreateContent(DoverStreetMarketLondonSubmitPayload payload, CancellationToken ct)
    {
      if (payload.Variant != null)
      {
        var contentWithVariant = await CreateContentWithVariant(payload, ct);
        return contentWithVariant;
      }

      var contentWithoutVariant = await CreateContentWithoutVariant(payload);
      return contentWithoutVariant;
    }

    private async Task<MultipartFormDataContent> CreateContentWithVariant(DoverStreetMarketLondonSubmitPayload payload, CancellationToken ct)
    {
      var formBoundary = await _stringUtils.GenerateRandomStringAsync(16);

      var country = _countriesService.GetCountryName(payload.Profile.CountryId)
        .Replace(" (" + payload.Profile.CountryId + ")", "");
      
      var content = new MultipartFormDataContent("----WebKitFormBoundary" + formBoundary)
      {
        {new StringContent(payload.ParsedRaffle.Form), "form"},
        {new StringContent(payload.ParsedRaffle.Viewkey), "viewkey"},
        {new StringContent(""), "password"},
        {new StringContent(payload.ParsedRaffle.HiddenFields), "hidden_fields"},
        {new StringContent(""), "incomplete"},
        {new StringContent(""), "incomplete_password"},
        {new StringContent("https://" + payload.ParsedRaffle.BaseUrl), "referrer"},
        {new StringContent("js"), "referrer_type"},
        {new StringContent("1"), "_submit"},
        {new StringContent("3"), "style_version"},
        {new StringContent(payload.ParsedRaffle.Viewparam), "viewparam"},
        {new StringContent(payload.Profile.FullName.Value), payload.ParsedRaffle.FullnameField},
        {new StringContent(payload.Email), payload.ParsedRaffle.EmailField},
        {new StringContent(payload.Profile.PhoneNumber.Value), payload.ParsedRaffle.PhoneNumberField},
        {new StringContent(payload.Variant), payload.ParsedRaffle.ColourField},
        {new StringContent(payload.SizeValue), payload.ParsedRaffle.SizeField},
        {new StringContent(payload.Profile.PostCode.Value), payload.ParsedRaffle.PostCodeField},
        {new StringContent(payload.Profile.AddressLine1.Value), payload.ParsedRaffle.AddressField},
        {new StringContent(country), payload.ParsedRaffle.CountryField},
        //{new StringContent(payload.Captcha), "g-recaptcha-response"},
        //{new StringContent(payload.Nonce), "nonce"}
      };
      
      if (payload.ShippingOption != null)
      {
        var questionAnswer = new StringContent(payload.ShippingOption);
        content.Add(questionAnswer, payload.ParsedRaffle.ShippingField);
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

    private async Task<MultipartFormDataContent> CreateContentWithoutVariant(DoverStreetMarketLondonSubmitPayload payload)
    {
      var formBoundary = await _stringUtils.GenerateRandomStringAsync(16);
      var country = _countriesService.GetCountryName(payload.Profile.CountryId)
        .Replace(" (" + payload.Profile.CountryId + ")", "");

      var content = new MultipartFormDataContent("----WebKitFormBoundary" + formBoundary)
      {
        {new StringContent(payload.ParsedRaffle.Form), "form"},
        {new StringContent(payload.ParsedRaffle.Viewkey), "viewkey"},
        {new StringContent(""), "password"},
        {new StringContent(""), "hidden_fields"},
        {new StringContent(""), "incomplete"},
        {new StringContent(""), "incomplete_password"},
        {new StringContent("https://" + payload.ParsedRaffle.BaseUrl), "referrer"},
        {new StringContent("js"), "referrer_type"},
        {new StringContent("1"), "_submit"},
        {new StringContent("3"), "style_version"},
        {new StringContent(payload.ParsedRaffle.Viewparam), "viewparam"},
        {new StringContent(payload.Profile.FullName), payload.ParsedRaffle.FullnameField},
        {new StringContent(payload.Email), payload.ParsedRaffle.EmailField},
        {new StringContent(payload.Profile.PhoneNumber), payload.ParsedRaffle.PhoneNumberField},
        {new StringContent(payload.SizeValue), payload.ParsedRaffle.SizeField},
        {new StringContent(payload.Profile.PostCode), payload.ParsedRaffle.PostCodeField},
        {new StringContent(payload.Profile.AddressLine1), payload.ParsedRaffle.AddressField},
        {new StringContent(country), payload.ParsedRaffle.CountryField},
       // {new StringContent(payload.Captcha), "g-recaptcha-response"},
       // {new StringContent(payload.Nonce), "nonce"}
      };
      
      if (payload.ShippingOption != null)
      {
        var questionAnswer = new StringContent(payload.ShippingOption);
        content.Add(questionAnswer, payload.ParsedRaffle.ShippingField);
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