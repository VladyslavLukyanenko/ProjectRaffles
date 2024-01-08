using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SnkrKuwaitModule
{
    public class SnkrKuwaitClient : ModuleHttpClientBase, ISnkrKuwaitClient
    {
        private readonly ICountriesService _countriesService;
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly IStringUtils _stringUtils;

        public SnkrKuwaitClient(ICountriesService countriesService, IStringUtils stringUtils)
        {
            _countriesService = countriesService;
            _stringUtils = stringUtils;
        }
        
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.AllowAutoRedirect = true;
            options.CookieContainer = _cookieContainer;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-dest","empty");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-mode","cors");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-site","same-origin");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }

        public async Task<string> GetRaffleProduct(string url, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(url, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            return node;
        }

        public async Task<SnkrKuwaitParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
        {
            var requestUrl = await HttpClient.GetAsync(raffleUrl, ct);
            var siteContent = await requestUrl.ReadStringResultOrFailAsync("Can't access page", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(siteContent);

            var formId = doc.DocumentNode.SelectSingleNode("//input[@name='form_id']").GetAttributeValue("value", "");

            var nameField = doc.DocumentNode.SelectSingleNode("//input[@title='Name']").GetAttributeValue("name", "");
            var emailField = doc.DocumentNode.SelectSingleNode("//input[@title='Email']").GetAttributeValue("name", "");
            var accountPasswordField = doc.DocumentNode.SelectSingleNode("//input[@title='Account Password']").GetAttributeValue("name", "");
            
            var genderField = doc.DocumentNode.SelectSingleNode("//select[@title='Gender']").GetAttributeValue("name", "");
            var ageGroupField = doc.DocumentNode.SelectSingleNode("//select[@title='Age Group']").GetAttributeValue("name", "");
           
            var dateOfBirthField = doc.DocumentNode.SelectSingleNode("//input[@title='Date of Birth']").GetAttributeValue("name", "");
            var mobileField = doc.DocumentNode.SelectSingleNode("//input[@title='Mobile Number']").GetAttributeValue("name", "");
            var idField = doc.DocumentNode.SelectSingleNode("//input[@title='ID Number']").GetAttributeValue("name", "");
            var streetAddressField = doc.DocumentNode.SelectSingleNode("//input[@title='Street Address']").GetAttributeValue("name", "");
            var addressLine2 = doc.DocumentNode.SelectSingleNode("//input[@title='Address line 2']").GetAttributeValue("name", "");
            var cityField = doc.DocumentNode.SelectSingleNode("//input[@title='City']").GetAttributeValue("name", "");
            
            var stateField = doc.DocumentNode.SelectSingleNode("//input[@title='State']").GetAttributeValue("name", "");
            var postField = doc.DocumentNode.SelectSingleNode("//input[@title='Postal / Zip Code']").GetAttributeValue("name", "");
            
            var countryField = doc.DocumentNode.SelectSingleNode("//select[@title='Country']").GetAttributeValue("name", "");
            var sizeField = doc.DocumentNode.SelectSingleNode("//select[@title='Shoe Size']").GetAttributeValue("name", "");
 
            return new SnkrKuwaitParsed(formId, nameField, emailField, accountPasswordField, genderField, ageGroupField,
                dateOfBirthField, mobileField, idField, streetAddressField, addressLine2, cityField, stateField,
                postField, countryField, sizeField);
        }

        public async Task<bool> SubmitAsync(AddressFields addressFields,SnkrKuwaitParsed parsed, Account account, string size, string raffleUrl,
            CancellationToken ct)
        {

            var getFormKey = await HttpClient.GetAsync(raffleUrl, ct);
            var siteBody = await getFormKey.ReadStringResultOrFailAsync("Can't access page", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(siteBody);
            
            var formkey =  doc.DocumentNode.SelectSingleNode("//input[@name='form_key']").GetAttributeValue("value", "");

            HttpClient.DefaultRequestHeaders.Add("origin","https://www.snkr.com.kw");
            HttpClient.DefaultRequestHeaders.Add("referer", raffleUrl);
            HttpClient.DefaultRequestHeaders.Add("x-requested-with","XMLHttpRequest");

            var address2 = "";
            if (addressFields.AddressLine2.Value != null) address2 = addressFields.AddressLine2.Value;
            
            var country = _countriesService.GetCountryName(addressFields.CountryId.Value)
                .Replace(" (" + addressFields.CountryId.Value + ")", "");

            var formBoundary = await _stringUtils.GenerateRandomStringAsync(16);
            
            var content = new MultipartFormDataContent("----WebKitFormBoundary" + formBoundary)
            {
                {new StringContent(parsed.FormId), "form_id"},
                {new StringContent(addressFields.FullName.Value), parsed.NameField},
                {new StringContent(account.Email), parsed.EmailField},
                {new StringContent(account.Password), parsed.AccountPasswordField},
                {new StringContent(""), parsed.GenderField},
                {new StringContent(""), parsed.AgeGroupField},
                {new StringContent(""), parsed.DateOfBirthField},
                {new StringContent(addressFields.PhoneNumber.Value), parsed.MobileField},
                {new StringContent(""), parsed.IdField},
                {new StringContent(addressFields.AddressLine1.Value), parsed.AddressLine1Field},
                {new StringContent(address2), parsed.AddressLine2Field},
                {new StringContent(addressFields.City.Value), parsed.CityField},
                {new StringContent(addressFields.ProvinceId.Value), parsed.StateField},
                {new StringContent(addressFields.PostCode.Value), parsed.PostField},
                {new StringContent(country), parsed.CountryField},
                {new StringContent(size), parsed.SizeField},
                {new StringContent(formkey), "form_key"}
            };

            HttpClient.DefaultRequestHeaders.Add("accept","*/*");
            var endpoint = "https://www.snkr.com.kw/form_builder/form/submit/";
            var postContent = await HttpClient.PostAsync(endpoint, content, ct);
            var postResponse = await postContent.ReadStringResultOrFailAsync("Error on submission");

            return postResponse.Contains(@"""success"":true");
        }

    }
}