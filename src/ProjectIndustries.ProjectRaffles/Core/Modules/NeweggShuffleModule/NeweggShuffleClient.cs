using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Elastic.Apm.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.NeweggAccountGenerator;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NeweggShuffleModule
{
    public class NeweggShuffleClient : ModuleHttpClientBase, INeweggShuffleClient
    {
        private CookieContainer _cookieContainer = new CookieContainer();
        
        private readonly string _publicKeySignature = @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA0YEMbfbEuUrYV9Y8CrPwcfz8O2AbRif5kSZddMSfZdrkvLYN/u+NxJbNyWshbO0KJGRH/Dm5RXEwBjGbYb0nf9vUCrAr28xkOwb+CbAMVrIEMmvwqir+Do7PVW0g+bJ0ROvX09wiW7pLS887AjA43jGE2F1wwOv3EqdYhX3eaIniuMKAmLIEvBXpS9ZtJAJL9lB6bfSMkUiwPKwSzzMGbDq689Kp7WuZzoKgryTSLPMaU3EvTav2R/+H12UxQsZGnPQ2JDsFUJIBdt7Es5wIKJuxuMP8EbfG47eB4ns56iCmg5Gf+9u0yBXwJZVXzrRaRpzMSjt4jL1j6BUQYlMO6wIDAQAB";

        private readonly string _publicKeyPassword =
            @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnMoWEfo0MxvCGBFL/XBY0qHGlbo83tJC+SgDhAf2lKCD8f+LqnsncA7NPmpz36RXwR/9vEKc4Op0TqFGTdI2c5pUYhdpBw6HNOLYwRvYK8AkfqxOCe88mwohsBpg3rgup+dIOc81cg9mDTAdGSbkaKye1w8AlSYqJDxVkl4e8W9ZoPRtNDcnfO8qAvwpQJ25iNGD62hwl7IqeBgSk00mNQsq96SmSqI58hYwKqN3nKXW5Q7MS0sYByuAC24BYDBTHaE0tGbSkdiHz0aQzvdqjOI380xQVq55AlEtryz0rnao7vEBOvYtjjMFuSlujeWMO7ij3RPXi0oBtKufjc3WYwIDAQAB";

        private readonly INeweggServices _neweggServices = new NeweggServices();
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.AllowAutoRedirect = true;
            options.CookieContainer = _cookieContainer;
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
            await HttpClient.GetAsync("https://www.newegg.com/", ct); //cookies

            var getAccountLoginPage = await HttpClient.GetAsync("https://secure.newegg.com/NewMyAccount/AccountLogin.aspx?nextpage=https%3A%2F%2Fwww.newegg.com%2F", ct);
            
            if (getAccountLoginPage.RequestMessage != null)
            {
                string responseUri = getAccountLoginPage.RequestMessage.RequestUri.ToString();
                return responseUri.Replace("https://secure.newegg.com/identity/signin?tk=", "");
            }

            throw new InvalidOperationException("Can't access signup page");
        }
        
        public async Task<NeweggFormValues> ParseData(string ticketKey, CancellationToken ct)
        {
            var getTokenSite = await HttpClient.GetAsync($"https://secure.newegg.com/identity/api/InitSignIn?ticket={ticketKey}", ct); //cookies

            var getSignupPage = await HttpClient.GetAsync($"https://secure.newegg.com/identity/signin?tk={ticketKey}", ct);
            var signupPageHtml = await getSignupPage.Content.ReadAsStringAsync(ct);
            
            var jsonRegex = new Regex(@"window\.__initialState__.*}<\/script><script defer="""">window.__neweggSt").Match(signupPageHtml).ToString().Replace("window.__initialState__ = ","").Replace(@"</script><script defer="""">window.__neweggSt","");

            dynamic parsedJson = JObject.Parse(jsonRegex);
            string fuzzyKey = parsedJson.resp.FuzzyChain;
            string emailKey = parsedJson.resp.SAL;
            string passwordKey = parsedJson.resp.PK;
            
            HttpClient.DefaultRequestHeaders.Remove("sec-fetch-dest");
            HttpClient.DefaultRequestHeaders.Remove("sec-fetch-mode");
            HttpClient.DefaultRequestHeaders.Remove("sec-fetch-site");
            HttpClient.DefaultRequestHeaders.Remove("accept");

            HttpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            HttpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            HttpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            HttpClient.DefaultRequestHeaders.Add("origin","https://secure.newegg.com");
            HttpClient.DefaultRequestHeaders.Add("referer", "https://secure.newegg.com/identity/signin?tk=" + ticketKey);
            HttpClient.DefaultRequestHeaders.Add("x-ua-pki","ne200508|ne200509");
            
            return new NeweggFormValues(null, null, emailKey, passwordKey, fuzzyKey);
        }

        public async Task LoginWithEmail(Account account, NeweggFormValues form, string ticketId, CancellationToken ct)
        {
            var passwordEncrypted = await _neweggServices.EncryptPayload(account.Password, _publicKeyPassword);
            
            Console.WriteLine("Encrypting payload");
            var signature = await _neweggServices.EncryptPayload($"S:SignIn:{account.Email}|{account.Password}", _publicKeySignature);
            
            Console.WriteLine("Encrypting accCertify");
            var accCertify = await _neweggServices.GetPayload("https://secure.newegg.com/identity/signup?tk=" + ticketId);

            var jsonToPost = new
            {
                emailKey = account.Email,
                passwordKey = passwordEncrypted,
                S = signature,
                FuzzyChain = form.FuzzyKey,
                AccertifyIdentityInfo = accCertify
            };
            
            var semiJson = JsonConvert.SerializeObject(jsonToPost);
            var replace = semiJson.Replace("emailKey", form.EmailKey).Replace("passwordKey", form.PasswordKey);
            var contentToPost = new StringContent(replace, Encoding.UTF8, "application/json");

            var loginUrl = "https://secure.newegg.com/identity/api/SignIn?ticket=" + ticketId;

            var postContent = await HttpClient.PostAsync(loginUrl, contentToPost, ct);
            var response = await postContent.Content.ReadAsStringAsync(ct);

            dynamic responseJson = JObject.Parse(response);
            string result = responseJson.Result;

            if (result != "Success")
            {
                throw new RaffleFailedException("Cannot login", "Cannot login: " + response);
            }
        }

        public Task LoginWithCookies(Account account, CancellationToken ct)
        {
            char[] delimiter = { ';' };
            string[] cookieParts = account.AccessToken.Split(delimiter);

            foreach (var stringCookie in cookieParts)
            {
                var cookieAdd = new Cookie();
                char[] cookieDelimiter = { ':' };
                string[] cookieChopped = stringCookie.Split(cookieDelimiter);
                if (cookieChopped[0].IsNullOrEmpty()) continue;
                cookieAdd.Name = cookieChopped[0];
                cookieAdd.Value = cookieChopped[1];
                cookieAdd.Domain = "newegg.com";

                _cookieContainer.Add(cookieAdd);
            }

            return Task.CompletedTask;
        }

        public async Task<string> GetLotteryId(CancellationToken ct)
        {
            var productShuffleUrl = "https://www.newegg.com/product-shuffle";

            var getProductShuffle = await HttpClient.GetAsync(productShuffleUrl, ct);
            var productShuffleResponse =
                await getProductShuffle.ReadStringResultOrFailAsync("Cannot access product shuffle site", ct);
            
            var lotteryJsonRegex = new Regex(@"window.__initialState__ = {.*}<\/script><script defer="""">window.__neweggSt").Match(productShuffleResponse).ToString().Replace("window.__initialState__ = ","").Replace(@"</script><script defer="""">window.__neweggSt","");;

            dynamic lotteryJson = JObject.Parse(lotteryJsonRegex);
            string lotteryId = lotteryJson.lotteryData.LotteryID;

            return lotteryId;
        }

        public async Task<bool> SignupToShuffle(string shuffleId, string lotteryId, CancellationToken ct)
        {
            var jsonToSend = new
            {
                LoginToken = "",
                LotteryID = lotteryId,
                LotteryInfos = new[]
                {
                    new
                    {
                        ItemNumber = shuffleId
                    }
                },
                url = "https://www.newegg.com/product-shuffle"
            };
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
            var serializeJson = JsonConvert.SerializeObject(jsonToSend, settings);
            var content = new StringContent(serializeJson, Encoding.UTF8, "application/json");

            HttpClient.DefaultRequestHeaders.Remove("referer");
            HttpClient.DefaultRequestHeaders.Remove("origin");
            HttpClient.DefaultRequestHeaders.Remove("x-ua-pki");
            
            HttpClient.DefaultRequestHeaders.Add("origin","https://www.newegg.com");
            HttpClient.DefaultRequestHeaders.Add("referer","https://www.newegg.com/product-shuffle");

            var postRegistration =
                await HttpClient.PostAsync("https://www.newegg.com/events/api/GoogleRecaptCha", content, ct);
            var response = await postRegistration.ReadStringResultOrFailAsync("Cannot post registration", ct);
            dynamic responseJson = JObject.Parse(response);
            string result = responseJson.Result;
            if(result != "Success") throw new RaffleFailedException("Cannot enter shuffle", "Cannot enter shuffle, response: " + response);

            return result.Equals("Success");
        }
    }
}