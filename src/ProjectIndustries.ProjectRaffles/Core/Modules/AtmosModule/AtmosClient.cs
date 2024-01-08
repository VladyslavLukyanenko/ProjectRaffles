using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AtmosModule
{
    public class AtmosClient : ModuleHttpClientBase, IAtmosClient
    {
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Accept","application/json, text/plain, */*");
                httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("Connection","keep-alive");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","empty");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","cors");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","cross-site");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }

        public async Task<AtmosParsed> ParseApiAsync(CancellationToken ct)
        {
            HttpClient.DefaultRequestHeaders.Add("Origin","https://releases.atmosusa.com");
            HttpClient.DefaultRequestHeaders.Add("Referer","https://releases.atmosusa.com/");

            var apiUrl = "https://stage-ubiq-raffle-strapi-be.herokuapp.com/releases/active";
            
            var getPage = await HttpClient.GetAsync(apiUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access releases", ct);
            
            HttpClient.DefaultRequestHeaders.Remove("Origin");
            HttpClient.DefaultRequestHeaders.Remove("Referer");

            dynamic parseApi = JArray.Parse(body);

            var models = new Dictionary<string, string>();
            var modelSizes = new Dictionary<string, List<string>>();
            var modelStores = new Dictionary<string, Dictionary<string, string>>();
            
            foreach (var raffleObject in parseApi)
            {
                string raffleObjectString = Convert.ToString(raffleObject);
                dynamic raffle = JObject.Parse(raffleObjectString);
                
                string raffleNameUpper = raffle.name;
                string raffleStyle = raffle.style;
                string completeRaffle = raffleNameUpper + " " + raffleStyle;
                
                string raffleName = completeRaffle.ToLower();
                string raffleId = raffle._id;
                models.Add(raffleName, raffleId);
                
                var sizeList = new List<string>();
                var storeDictionary = new Dictionary<string, string>();
                
                foreach (var sizeObject in raffle.sizes)
                {
                    string size = Convert.ToString(sizeObject);
                    sizeList.Add(size);
                }
                foreach (var storeObject in raffle.stores)
                {
                    string storeObjectString = Convert.ToString(storeObject);
                    dynamic stores = JObject.Parse(storeObjectString);

                    string storeName = stores.name;
                    string storeId = stores._id;
                    storeDictionary.Add(storeName.ToLower(), storeId);
                }
                
                modelSizes.Add(raffleId, sizeList);
                modelStores.Add(raffleId, storeDictionary);
            }
            
            return new AtmosParsed(models, modelSizes, modelStores);
        }

        public Task<AtmosSelectedProduct> GetSizeAndModelAsync(AtmosParsed parsed, string model, string size, string store)
        {

            var sizeLookUp = size.Replace(",", "_").Replace(".", "_");

            //get modelId, since there can be multiple models with same name, we use the ID
            parsed.Models.TryGetValue(model, out string modelId);
            
            //get list of sizes according to model ID
            parsed.ModelSizes.TryGetValue(modelId, out List<string> sizeList);

            var foundSize = "";
            if (size.ToLower() == "random")
            {
                var rnd = new Random();
                if (sizeList != null)
                {
                    var sizeListCount = sizeList.Count;
                    var randomListItem = rnd.Next(sizeListCount);
                    foundSize = sizeList[randomListItem];
                }
            }
            else
            {
                if (sizeList != null) foundSize = sizeList.Find(x => x == sizeLookUp);
            }

            parsed.ModelStores.TryGetValue(modelId, out Dictionary<string, string> modelDictionary);

            var storeId = "";
            modelDictionary?.TryGetValue(store.ToLower(), out storeId);

            return Task.FromResult(new AtmosSelectedProduct(modelId, foundSize, storeId));
        }

        public async Task<bool> SubmitAsync(AddressFields addressFields, AtmosSelectedProduct product, string captcha, string email, string instagram, string useAddress, CancellationToken ct)
        {
            HttpClient.DefaultRequestHeaders.Add("Origin","https://releases.atmosusa.com");
            HttpClient.DefaultRequestHeaders.Add("Referer","https://releases.atmosusa.com/");
            
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
            AtmosJson json;
            if (useAddress == "Y")
            {
                json = new AtmosJson
                {
                    release = product.ReleaseId,
                    size = product.Size,
                    email = email,
                    confirmEmail = email,
                    firstName = addressFields.FirstName.Value,
                    lastName = addressFields.LastName.Value,
                    zipCode = addressFields.PostCode.Value,
                    instagramHandle = instagram,
                    phoneNumber = addressFields.PhoneNumber.Value,
                    recaptcha = captcha,
                    address1 = addressFields.AddressLine1.Value,
                    address2 = addressFields.AddressLine2.Value,
                    state = addressFields.ProvinceId.Value,
                    city = addressFields.City.Value,
                    store = product.StoreId
                };
            }
            else
            {
                json = new AtmosJson
                {
                    release = product.ReleaseId,
                    size = product.Size,
                    email = email,
                    confirmEmail = email,
                    firstName = addressFields.FirstName.Value,
                    lastName = addressFields.LastName.Value,
                    zipCode = addressFields.PostCode.Value,
                    instagramHandle = instagram,
                    phoneNumber = addressFields.PhoneNumber.Value,
                    recaptcha = captcha,
                    store = product.StoreId
                };
            }
            var serializedContent = JsonConvert.SerializeObject(json, settings);

            var content = new StringContent(serializedContent, Encoding.UTF8, "application/json");

            var endpoint = "https://stage-ubiq-raffle-strapi-be.herokuapp.com/entries";
            var postEntry = await HttpClient.PostAsync(endpoint, content, ct);
            if (!postEntry.IsSuccessStatusCode) await postEntry.FailWithRootCauseAsync("Error on submission", ct);

            return postEntry.IsSuccessStatusCode;
        }
    }
}