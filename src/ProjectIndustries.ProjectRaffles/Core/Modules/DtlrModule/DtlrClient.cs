using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DtlrModule
{
    public class DtlrClient : ModuleHttpClientBase, IDtlrClient
    {
        private readonly IBirthdayProviderService _birthdayProvider;
        private readonly ICountriesService _countriesService;
        private readonly IStringUtils _stringUtils;

        public DtlrClient(IBirthdayProviderService birthdayProvider, ICountriesService countriesService, IStringUtils stringUtils)
        {
            _birthdayProvider = birthdayProvider;
            _countriesService = countriesService;
            _stringUtils = stringUtils;
        }
        
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                httpClient.DefaultRequestHeaders.Add("Connection","keep-alive");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","empty");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","cors");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","cross-site");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }

        public async Task<string> GetProductAsync(string raffleurl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleurl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
            return node;
        }

        public async Task<DtlrParsed> ParseRaffleAsync(string raffleurl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleurl, ct);
            var pageContent = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var gFormId = doc.DocumentNode.SelectSingleNode("//input[@name='gform_submit']").GetAttributeValue("value","");
            
            var firstNameInput = doc.DocumentNode.SelectSingleNode("//input[@placeholder='First Name']").GetAttributeValue("name","");
            var lastNameInput = doc.DocumentNode.SelectSingleNode("//input[@placeholder='Last Name']").GetAttributeValue("name","");
            
            var emailInput = doc.DocumentNode.SelectSingleNode("//input[@placeholder='Email Address']").GetAttributeValue("name","");
            var verifyEmailInput = doc.DocumentNode.SelectSingleNode("//input[@placeholder='Email Address (verify)']").GetAttributeValue("name","");

            var birthYearPattern = @"input_" + gFormId + @"_\d{1,3}'>Birth";
            var birthYearInput = new Regex(birthYearPattern).Match(pageContent).ToString().Replace("_"+gFormId,"").Replace(@"'>Birth","");
            
            var phoneNumberInput = doc.DocumentNode.SelectSingleNode("//input[@placeholder='Mobile Number']").GetAttributeValue("name","");
            
            var addressLine1Input = doc.DocumentNode.SelectSingleNode("//input[@placeholder='Street Address']").GetAttributeValue("name","");
            var cityInput = doc.DocumentNode.SelectSingleNode("//input[@placeholder='City']").GetAttributeValue("name","");

            var stateInput = addressLine1Input.Replace(".1", ".4");
            
            var postCodeInput = doc.DocumentNode.SelectSingleNode("//input[@placeholder='ZIP']").GetAttributeValue("name","");
            var countryInput = doc.DocumentNode.SelectSingleNode("//input[@value='United States']").GetAttributeValue("name","");
            
            var genderInput = doc.DocumentNode.SelectSingleNode("//input[@value='Male']").GetAttributeValue("name","");

            var sizeInputPattern = @"input_" + gFormId + @"_\d{1,3}'>Select";
            var sizeInput = new Regex(sizeInputPattern).Match(pageContent).ToString().Replace("_"+gFormId,"").Replace(@"'>Select","");
            
            
            var valueLessInput19 = doc.DocumentNode.SelectSingleNode("//input[@name='input_19']").GetAttributeValue("value","");
            var valueLessInput20 = doc.DocumentNode.SelectSingleNode("//input[@name='input_20']").GetAttributeValue("value","");
            
            var gformUniqueId = doc.DocumentNode.SelectSingleNode("//input[@name='gform_unique_id']").GetAttributeValue("value","");
            var formState = doc.DocumentNode.SelectSingleNode($"//input[@name='state_{gFormId}']").GetAttributeValue("value","");
            var formTargetPage = doc.DocumentNode.SelectSingleNode($"//input[@name='gform_target_page_number_{gFormId}']").GetAttributeValue("value","");
            var formSourcePage = doc.DocumentNode.SelectSingleNode($"//input[@name='gform_source_page_number_{gFormId}']").GetAttributeValue("value","");
            var fieldValues = doc.DocumentNode.SelectSingleNode("//input[@name='gform_field_values']").GetAttributeValue("value","");

            return new DtlrParsed(gFormId, firstNameInput, lastNameInput, emailInput, verifyEmailInput, birthYearInput,
                phoneNumberInput, addressLine1Input, cityInput, stateInput, postCodeInput, countryInput, genderInput,
                sizeInput, valueLessInput19, valueLessInput20, gformUniqueId, formState, formTargetPage, formSourcePage, fieldValues);
        }

        public async Task<bool> SubmitAsync(AddressFields addressFields, DtlrParsed parsed, string email,
            string raffleUrl, string size, string captcha, CancellationToken ct)
        {
            var birthYear = await _birthdayProvider.GetYear();

            var formBoundary = await _stringUtils.GenerateRandomStringAsync(16);
            
            var state = _countriesService.GetProvinceName("US",
                addressFields.ProvinceId.Value);

            var content = new MultipartFormDataContent("----WebKitFormBoundary" + formBoundary)
            {
                {new StringContent(addressFields.FirstName.Value), parsed.FirstNameInput},
                {new StringContent(addressFields.LastName.Value), parsed.LastNameInput},
                {new StringContent(email), parsed.EmailInput},
                {new StringContent(email), parsed.VerifyEmailInput},
                {new StringContent(birthYear.ToString()), parsed.BirthYearInput},
                {new StringContent(addressFields.PhoneNumber.Value), parsed.PhoneNumberInput},
                {new StringContent(addressFields.AddressLine1.Value), parsed.AddressLine1Input},
                {new StringContent(addressFields.City.Value), parsed.CityInput},
                {new StringContent(state), parsed.StateInput},
                {new StringContent(addressFields.PostCode.Value), parsed.PostCodeInput},
                {new StringContent("United States"), parsed.CountryInput},
                {new StringContent("Male"), parsed.GenderInput},
                {new StringContent(size), parsed.SizeInput},
                {new StringContent(parsed.ValueLessInput19), "input_19"},
                {new StringContent(captcha), "g-recaptcha-response"},
                {new StringContent(parsed.ValueLessInput20), "input_20"},
                {new StringContent("1"), $"is_submit_{parsed.FormId}"},
                {new StringContent(parsed.FormId), "gform_submit"},
                {new StringContent(parsed.gFormUnique), "gform_unique_id"},
                {new StringContent(parsed.StateInput), $"state_{parsed.FormId}"},
                {new StringContent(parsed.TargetPage), $"gform_target_page_number_{parsed.FormId}"},
                {new StringContent(parsed.SourcePage), $"gform_source_page_number_{parsed.FormId}"},
                {new StringContent(parsed.FieldValues), "gform_field_values"}
            };
            
            HttpClient.DefaultRequestHeaders.Add("Origin","https://blog.dtlr.com");
            HttpClient.DefaultRequestHeaders.Add("Referer",raffleUrl);

            var post = await HttpClient.PostAsync(raffleUrl, content, ct);
            var postResponse = await post.ReadStringResultOrFailAsync("Error on submission", ct);

            if (!postResponse.Contains("Click the button in the email"))
                await post.FailWithRootCauseAsync($"Error on submitting with email {email}", ct);
            
            return postResponse.Contains("Click the button in the email");
        }
    }
}