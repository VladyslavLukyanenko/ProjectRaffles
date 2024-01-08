using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AlleyoopModule
{
    public class AlleyoopClient : ModuleHttpClientBase, IAlleyoopClient
    {
        private readonly IBirthdayProviderService _birthdayProvider;
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly IStringUtils _stringUtils;

        public AlleyoopClient(IBirthdayProviderService birthdayProvider, IStringUtils stringUtils)
        {
            _birthdayProvider = birthdayProvider;
            _stringUtils = stringUtils;
        }
        
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.CookieContainer = _cookieContainer;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("Connection","keep-alive");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","empty");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","cors");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","cross-site");
                httpClient.DefaultRequestHeaders.Add("sec-gpc","1");
            };
        }

        public async Task<string> GetProductCode(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var pageContent = await getPage.ReadStringResultOrFailAsync("Can't access form", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var itemCode = doc.DocumentNode.SelectSingleNode("//input[@name='item_code']")
                .GetAttributeValue("value", "");

            return itemCode;
        }

        public async Task<bool> SubmitAsync(AlleyoopSubmitPayload payload, CancellationToken ct)
        {
            var formBoundary = await _stringUtils.GenerateRandomStringAsync(16);

            var age = await _birthdayProvider.GenerateAge();

            await HttpClient.GetAsync(payload.RaffleUrl, ct); //for getting PHPSESSID cookie

            var content = new MultipartFormDataContent("----WebKitFormBoundary" + formBoundary)
            {
                {new StringContent("on"), "save"},
                {new StringContent(payload.ItemCode), "item_code"},
                {new StringContent(payload.AddressFields.LastName.Value), "family_name"},
                {new StringContent(payload.AddressFields.FirstName.Value), "given_name"},
                {new StringContent(payload.FuriganaLast), "family_name_kana"},
                {new StringContent(payload.FuriganaFirst), "given_name_kana"},
                {new StringContent(age.ToString()), "nenrei"},
                {new StringContent(payload.AddressFields.PhoneNumber.Value), "tel"},
                {new StringContent(payload.Email), "mail"},
                {new StringContent(payload.AddressFields.PostCode.Value), "zipcode"},
                {new StringContent(payload.AddressFields.ProvinceId.Value), "address1"},
                {new StringContent(payload.AddressFields.City.Value), "address2"},
                {new StringContent(payload.Building), "address3"}, //seems to be "building"
                {new StringContent(payload.Payment), "payment"},
                {new StringContent(payload.DeliveryTime), "deliveryTime"}
            };

            var endpoint = "https://alleyoop.shop/form2.php";
            var postContent = await HttpClient.PostAsync(endpoint, content, ct);
            if (!postContent.IsSuccessStatusCode) await postContent.FailWithRootCauseAsync("Error on submission", ct);

            return postContent.IsSuccessStatusCode;
        }
    }
}