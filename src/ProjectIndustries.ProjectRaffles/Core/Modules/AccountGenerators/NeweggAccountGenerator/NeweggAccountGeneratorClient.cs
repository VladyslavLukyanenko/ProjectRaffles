using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.NeweggAccountGenerator
{
    public class NeweggAccountGeneratorClient : INeweggAccountGeneratorClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly IHttpClientBuilder _builder;
        private HttpClient _httpClient;
        private readonly INeweggServices _neweggServices = new NeweggServices();

        private readonly string _publicKeySignature = @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA0YEMbfbEuUrYV9Y8CrPwcfz8O2AbRif5kSZddMSfZdrkvLYN/u+NxJbNyWshbO0KJGRH/Dm5RXEwBjGbYb0nf9vUCrAr28xkOwb+CbAMVrIEMmvwqir+Do7PVW0g+bJ0ROvX09wiW7pLS887AjA43jGE2F1wwOv3EqdYhX3eaIniuMKAmLIEvBXpS9ZtJAJL9lB6bfSMkUiwPKwSzzMGbDq689Kp7WuZzoKgryTSLPMaU3EvTav2R/+H12UxQsZGnPQ2JDsFUJIBdt7Es5wIKJuxuMP8EbfG47eB4ns56iCmg5Gf+9u0yBXwJZVXzrRaRpzMSjt4jL1j6BUQYlMO6wIDAQAB";

        private readonly string _publicKeyPassword =
            @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnMoWEfo0MxvCGBFL/XBY0qHGlbo83tJC+SgDhAf2lKCD8f+LqnsncA7NPmpz36RXwR/9vEKc4Op0TqFGTdI2c5pUYhdpBw6HNOLYwRvYK8AkfqxOCe88mwohsBpg3rgup+dIOc81cg9mDTAdGSbkaKye1w8AlSYqJDxVkl4e8W9ZoPRtNDcnfO8qAvwpQJ25iNGD62hwl7IqeBgSk00mNQsq96SmSqI58hYwKqN3nKXW5Q7MS0sYByuAC24BYDBTHaE0tGbSkdiHz0aQzvdqjOI380xQVq55AlEtryz0rnao7vEBOvYtjjMFuSlujeWMO7ij3RPXi0oBtKufjc3WYwIDAQAB";

        
        public NeweggAccountGeneratorClient(IHttpClientBuilder builder)
        {
            _builder = builder;
        }
        
        public void GenerateHttpClient()
        {
            _httpClient = _builder.WithConfiguration(ConfigureHttpClient)
                .Build();
        }
        
        private void ConfigureHttpClient(HttpClientOptions options)
        {
            options.CookieContainer = _cookieContainer;
            options.AllowAutoRedirect = true;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                httpClient.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua",@"""Google Chrome"";v=""89"", ""Chromium"";v=""89"", "";Not A Brand"";v=""99""");
                httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile","?0");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "none");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-user","?1");
                httpClient.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
            };
        }
        
        public async Task<string> GetTicketId(CancellationToken ct)
        {
            await _httpClient.GetAsync("https://www.newegg.com/", ct); //cookies

            var getAccountLoginPage = await _httpClient.GetAsync("https://secure.newegg.com/NewMyAccount/AccountLogin.aspx?nextpage=https%3A%2F%2Fwww.newegg.com%2F", ct);
            
            if (getAccountLoginPage.RequestMessage != null)
            {
                string responseUri = getAccountLoginPage.RequestMessage.RequestUri.ToString();
                return responseUri.Replace("https://secure.newegg.com/identity/signin?tk=", "");
            }

            throw new InvalidOperationException("Can't access signup page");
        }

        public async Task<NeweggFormValues> ParseData(string ticketId, CancellationToken ct)
        {
             await _httpClient.GetAsync($"https://secure.newegg.com/identity/api/InitSignUp?ticket={ticketId}", ct); //cookies

            var getSignupPage = await _httpClient.GetAsync($"https://secure.newegg.com/identity/signup?tk={ticketId}", ct);
            var signupPageHtml = await getSignupPage.Content.ReadAsStringAsync(ct);

            var firstNameRegex = new Regex(@"""labeled-input-.{1,50}"">First Name").Match(signupPageHtml).ToString().Replace(@"""","").Replace("labeled-input-","").Replace(">First Name","");
            var lastNameRegex = new Regex(@"""labeled-input-.{1,50}"">Last Name").Match(signupPageHtml).ToString().Replace(@"""","").Replace("labeled-input-","").Replace(">Last Name","");
            var emailRegex = new Regex(@"""labeled-input-.{1,50}"">Email Address").Match(signupPageHtml).ToString().Replace(@"""","").Replace("labeled-input-","").Replace(">Email Address","");
            var passwordRegex = new Regex(@"""labeled-input-.{1,50}"">Password").Match(signupPageHtml).ToString().Replace(@"""","").Replace("labeled-input-","").Replace(">Password","");

            var jsonRegex = new Regex(@"window\.__initialState__.*}<\/script><script defer="""">window.__neweggSt").Match(signupPageHtml).ToString().Replace("window.__initialState__ = ","").Replace(@"</script><script defer="""">window.__neweggSt","");

            dynamic parsedJson = JObject.Parse(jsonRegex);
            string fuzzyKey = parsedJson.resp.FuzzyChain;
            
            _httpClient.DefaultRequestHeaders.Remove("sec-fetch-dest");
            _httpClient.DefaultRequestHeaders.Remove("sec-fetch-mode");
            _httpClient.DefaultRequestHeaders.Remove("sec-fetch-site");
            _httpClient.DefaultRequestHeaders.Remove("accept");

            _httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            _httpClient.DefaultRequestHeaders.Add("origin","https://secure.newegg.com");
            _httpClient.DefaultRequestHeaders.Add("referer", "https://secure.newegg.com/identity/signup?tk=" + ticketId);
            _httpClient.DefaultRequestHeaders.Add("x-ua-pki","ne200508|ne200509");
            
            return new NeweggFormValues(firstNameRegex, lastNameRegex, emailRegex, passwordRegex, fuzzyKey);
        }

        public async Task<dynamic> SubmitAccount(AddressFields addressFields, string email, string captcha, NeweggFormValues formValues, string ticketId, CancellationToken ct)
        {
            var rnd = new Random();
            var phoneNumber = $"{rnd.Next(0, 9)}{rnd.Next(0, 9)}{rnd.Next(0, 9)}{rnd.Next(0, 9)}{rnd.Next(0, 9)}{rnd.Next(0, 9)}{rnd.Next(0, 9)}{rnd.Next(0, 9)}{rnd.Next(0, 9)}{rnd.Next(0, 9)}";
            
            var password = await _neweggServices.EncryptPayload("ProjectRaffles123!", _publicKeyPassword);
            
            var signature = await _neweggServices.EncryptPayload($"S:SignUp:{email}|ProjectRaffles123!", _publicKeySignature);
            
            var accCertify = await _neweggServices.GetPayload("https://secure.newegg.com/identity/signup?tk=" + ticketId);

            string firstName = addressFields.FirstName.Value;
            string lastName = addressFields.LastName.Value;
            var firstJson = new
            {
                firstNameReplace = firstName,
                lastNameReplace = lastName,
                emailReplace = email.ToLower(),
                MobilePhoneNumber = phoneNumber,
                passwordKeyReplace = password,
                AllowEmail = false,
                RecaptchaResponse = captcha,
                TextCode = "",
                FuzzyChain = formValues.FuzzyKey,
                S = signature,
                AccertifyIdentityInfo = accCertify
            };
            
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
            
            var semiJson = JsonConvert.SerializeObject(firstJson, settings);
            var replace = semiJson.Replace("firstNameReplace", formValues.FirstNameKey)
                .Replace("lastNameReplace", formValues.LastNameKey).Replace("emailReplace", formValues.EmailKey)
                .Replace("passwordKeyReplace", formValues.PasswordKey);
            var contentToPost = new StringContent(replace, Encoding.UTF8, "application/json");

            var createAccountUrl = "https://secure.newegg.com/identity/api/SignUp?ticket=" + ticketId;
            
            var postContent = await _httpClient.PostAsync(createAccountUrl, contentToPost, ct);
            var response = await postContent.ReadStringResultOrFailAsync("Error on posting account: " + postContent, ct);

            dynamic responseJson = JObject.Parse(response);

            return responseJson;
        }
        
        public async Task<string> GetAccountCookies(dynamic response, CancellationToken ct) //save them to be used as an alternative to logging in. more reliable
        {
            string callbackUrl = response.CallbackPage;

            var getCallback = await _httpClient.GetAsync(callbackUrl, ct);
            var callbackResponse = await getCallback.Content.ReadAsStringAsync(ct);

            var cookieString = "";
            
            foreach (Cookie cookie in _cookieContainer.GetCookies(new Uri("https://www.newegg.com")))
            {
                string cookieName = cookie.Name;
                string cookieValue = cookie.Value;

                string addCookie = $"{cookieName}:{cookieValue};";
                cookieString += addCookie;
            }

            return cookieString;
        }
    }
}