using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WelcomeSkateModule
{
    public class WelcomeSkateClient : ModuleHttpClientBase, IWelcomeSkateClient
    {
        private readonly ICountriesService _countriesService;

        public WelcomeSkateClient(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }
        
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }

        public async Task<string> GetProductAsync(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access site", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;

            return node;
        }

        public async Task<WelcomeSkateParsed> GetIdxValuesAsync(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var pageContent = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);
            
            var formFinder = new HtmlDocument();
            formFinder.LoadHtml(pageContent);

            var formId = formFinder.DocumentNode.SelectSingleNode("//div[@class='powr-form-builder']")
                .GetAttributeValue("id", "");

            var formUrl = "https://www.powr.io/form-builder/u/" + formId;

            var getForm = await HttpClient.GetAsync(formUrl, ct);
            var formContent = await getForm.ReadStringResultOrFailAsync("Can't access form", ct);

            var appIdRegexPattern = @"window\.META={""id"":\d*";
            var appId = new Regex(appIdRegexPattern).Match(formContent).ToString().Replace(@"window.META={""id"":","");
            
            var formJsonRegex = @"window\.CONTENT=.*}";
            var formJsonString = new Regex(formJsonRegex).Match(formContent).ToString().Replace(@"window.CONTENT=","");

            dynamic formJson = JObject.Parse(formJsonString);
            
            var idxDictionary = new Dictionary<string, string>();

            foreach (var formField in formJson.data)
            {
                string label = formField.label;
                string loweredLabel = label.ToLower();
                string idx = formField.idx;

                if (string.IsNullOrEmpty(label)) label = "notNeeded";
                
                idxDictionary.Add(loweredLabel, idx);
            }
            
            return new WelcomeSkateParsed(idxDictionary, formId, appId);
        }

        public async Task<bool> SubmitAsync(AddressFields addressFields, WelcomeSkateParsed parsed, string email,
            string captcha,
            string size, CancellationToken ct)
        {
            var nameFieldLabel = "Nombre y Apellidos / Name and Last Name";
            var emailFieldLabel = "Email";
            var phoneFieldLabel = "Telefono / Phone";
            var countryFieldLabel = "Pais / Country";
            var sizeFieldLabel = "Talla /Size";
            var authorizeLabel =
                @"Autorizas a Welcome Skateboarding a salvar tus datos, que serán utilizados exclusivamente para mejorar tu experiencia de compra y enviarte nuevos productos y ofertas. / Authorization for email communication only about welcome skateboarding launch";
            
            
            parsed.IdxDictionary.TryGetValue(nameFieldLabel.ToLower(), out string nameIdx);
            parsed.IdxDictionary.TryGetValue(emailFieldLabel.ToLower(), out string emailIdx);
            parsed.IdxDictionary.TryGetValue(phoneFieldLabel.ToLower(), out string phoneIdx);
            parsed.IdxDictionary.TryGetValue(countryFieldLabel.ToLower(), out string countryIdx);
            parsed.IdxDictionary.TryGetValue(sizeFieldLabel.ToLower(), out string sizeIdx);
            parsed.IdxDictionary.TryGetValue(authorizeLabel.ToLower(), out string authorizeIdx);

            var country = _countriesService.GetCountryName(addressFields.CountryId.Value)
                .Replace(" (" + addressFields.CountryId.Value + ")", "");

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"appId", parsed.AppId}, //find
                {"data[0][label]", nameFieldLabel},
                {"data[0][idx]", nameIdx},
                {"data[0][type]", "text"},
                {"data[0][value]", addressFields.FullName.Value},
                
                {"data[1][label]", emailFieldLabel},
                {"data[1][idx]", emailIdx},
                {"data[1][type]", "email"},
                {"data[1][value]", email},
                
                {"data[2][label]", phoneFieldLabel},
                {"data[2][idx]", phoneIdx},
                {"data[2][type]", "text"},
                {"data[2][value]", addressFields.PhoneNumber.Value},
                
                {"data[3][label]", countryFieldLabel},
                {"data[3][idx]", countryIdx},
                {"data[3][type]", "select"},
                {"data[3][value]", country}, 
                
                {"data[4][label]", sizeFieldLabel},
                {"data[4][idx]", sizeIdx},
                {"data[4][type]", "select"},
                {"data[4][value]", size},
                
                {"data[5][label]", authorizeLabel},
                {"data[5][idx]", authorizeIdx},
                {"data[5][type]","multipleCheckbox"},
                {"data[5][value]","Autorizo / authorize"},
                {"data[5][selectedPrices][]","0"},
                
                {"confirmationEmail", email},
                {"responseToken",""},
                {"recaptchaToken", captcha},
                {"recaptchaType","v2_checkbox"}
            });

            HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","empty");
            HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","cors");
            HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","cross-site");
            
            var endpoint = $"https://www.powr.io/app_form_response/{parsed.RaffleId}.json";
            var post = await HttpClient.PostAsync(endpoint, content, ct);
            var postContent = await post.ReadStringResultOrFailAsync("Error on submission", ct);

            if (!postContent.Contains("true")) await post.FailWithRootCauseAsync($"Error with email {email}", ct);
            
            return postContent.Contains("true");
        }
    }
}