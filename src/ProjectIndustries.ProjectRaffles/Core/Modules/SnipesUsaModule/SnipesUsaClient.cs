using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SnipesUsaModule
{
    public class SnipesUsaClient : ModuleHttpClientBase, ISnipesUsaClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly IStringUtils _stringUtils;

        public SnipesUsaClient(IStringUtils stringUtils)
        {
            _stringUtils = stringUtils;
        }
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.CookieContainer = _cookieContainer;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
            };
        }
        
        public async Task<SnipesApiParsed> ParseSnipesApiAsync(CancellationToken ct)
        {
           //get cookies
           var releasePage = "https://raffle.snipesusa.com/releases/";
           var getCookies = await HttpClient.GetAsync(releasePage, ct); 
           if (!getCookies.IsSuccessStatusCode) await getCookies.FailWithRootCauseAsync("Can't access release page", ct);
            
           ////////
           var guid = Guid.NewGuid().ToString();
           HttpClient.DefaultRequestHeaders.Add("referer","https://raffle.snipesusa.com/releases/");
           HttpClient.DefaultRequestHeaders.Add("authority","raffle.snipesusa.com");
           HttpClient.DefaultRequestHeaders.Add("x-csrf-token", guid);

           var unix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

           var releasesUrl = "https://raffle.snipesusa.com/api/releases/current?no-cache=" + unix;

           var getReleases = await HttpClient.GetAsync(releasesUrl, ct);
           var allReleases = await getReleases.ReadStringResultOrFailAsync("Can't get releases", ct);
           
           dynamic parseReleases = JObject.Parse(allReleases);
           var allModels = parseReleases.data;

           //contains all models + all sizes, visualize like <modelName,<size,<storeName,storeSizeId>>>
           var modelSizesStoresDictionary = new Dictionary<string, Dictionary<string, Dictionary<string,string>>>();
           foreach (var model in allModels)
           { 
               var allSizes = model.sizes;
                
               string modelNameString = model.name;
               string modelName = modelNameString.ToLower();

               //first string is size, second dictionary is <storename,storeSizeID>, so it's <size,<storename,storeSizeId>>
               var allStoreSizes = new Dictionary<string, Dictionary<string,string>>();  
               foreach (var sizeObject in allSizes)
               { 
                   //<storeName,storeSizeId>
                   var storeModelDictionary = new Dictionary<string, string>();   
                    
                   string size = sizeObject.size;
                   foreach (var storeName in sizeObject.stores)
                   { 
                       string storeString = storeName.name;
                       string modelSizeIdString = storeName.quantity_id;

                       string store = storeString.ToLower();
                       string modelSizeId = modelSizeIdString.ToLower();
                       storeModelDictionary.Add(store, modelSizeId);
                   }
                   allStoreSizes.Add(size, storeModelDictionary);
               }

               modelSizesStoresDictionary.Add(modelName, allStoreSizes);
           }

           HttpClient.DefaultRequestHeaders.Remove("referer");
           HttpClient.DefaultRequestHeaders.Remove("authority");
           HttpClient.DefaultRequestHeaders.Remove("x-csrf-token");
           
           return new SnipesApiParsed(modelSizesStoresDictionary); 
        }

        public Task<string> GetSizeIdAsync(SnipesApiParsed parsedApi, string userModel, string userSize, string userStore)
        {
            var model = userModel.ToLower();
            var store = "snipes - " + userStore.ToLower();
            
            parsedApi.SnipesApiDictionary.TryGetValue(model, out Dictionary<string, Dictionary<string, string>> sizeDictionary);
            if(sizeDictionary == null) throw new RaffleFailedException("Can't find model in dictionary","Can't find model");
            
            sizeDictionary.TryGetValue(userSize, out Dictionary<string, string> storeDictionary);
            if(storeDictionary == null) throw new RaffleFailedException("Can't find size in dictionary","Can't find sizes");

            storeDictionary.TryGetValue(store, out string sizeId);
            if(string.IsNullOrEmpty(sizeId)) throw new RaffleFailedException($"Size ID null or empty for store: {store} and size: {userSize}",$"Can't find size ID for store {store}");

            return Task.FromResult(sizeId);
        }

        public async Task<bool> SubmitAsync(AddressFields addressFields, string userEmail, string sizeGuid, string captcha,
            CancellationToken ct)
        {

            //getting _csrf cookie + the header:
            var getSite = await HttpClient.GetAsync("https://raffle.snipesusa.com/releases/current", ct);
            var siteContent = await getSite.ReadStringResultOrFailAsync("Can't access release page", ct);
            var doc = new HtmlDocument();
            doc.LoadHtml(siteContent);
            var metaContent = doc.DocumentNode.SelectSingleNode("//meta[@name='config']").GetAttributeValue("content", "");
            var urlDecodeMetaContent = Uri.UnescapeDataString(metaContent);
            dynamic metaContentParsed = JObject.Parse(urlDecodeMetaContent);
            string csrfHeader = metaContentParsed.csrf;
            HttpClient.DefaultRequestHeaders.Add("x-csrf-token", csrfHeader);
            
            
            var rootContent = new
            {
                email = userEmail,
                first_name = addressFields.FirstName.Value,
                last_name = addressFields.LastName.Value,
                phone_number = addressFields.PhoneNumber.Value,
                postal_code = addressFields.PostCode.Value,
                quantity_id = sizeGuid,
                recaptcha = captcha
            };
            
            var rootSerialized = JsonConvert.SerializeObject(rootContent);
            var content = new StringContent(rootSerialized, Encoding.UTF8, "application/json");
            
            HttpClient.DefaultRequestHeaders.Add("referer","https://raffle.snipesusa.com/signup");
            HttpClient.DefaultRequestHeaders.Add("authority","raffle.snipesusa.com");

            var endpoint = "https://raffle.snipesusa.com/api/registrations";
            var postAsync = await HttpClient.PostAsync(endpoint, content, ct);
            
            var postContent = await postAsync.ReadStringResultOrFailAsync("Error on submission", ct);

            return postContent.Contains("confirmation_code");
        }
    }
}