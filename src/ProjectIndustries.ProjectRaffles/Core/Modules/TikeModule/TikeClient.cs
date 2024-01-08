using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.TikeModule
{
    public class TikeClient : ModuleHttpClientBase, ITikeClient
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.CookieContainer = _cookieContainer;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-User","?1");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site","same-origin");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode","navigate");
                httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest","document");
                httpClient.DefaultRequestHeaders.Add("Accept-Language","en-DK,en;q=0.9,da-DK;q=0.8,da;q=0.7,en-US;q=0.6");
                httpClient.DefaultRequestHeaders.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            };
        }
        
        public async Task<string> GetProductAsync(string raffleUrl, CancellationToken ct)
        {
            var getPage = await HttpClient.GetAsync(raffleUrl, ct);
            var body = await getPage.ReadStringResultOrFailAsync("Can't access page", ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectSingleNode("//head/title").InnerText;
            return node;
        }

        public async Task<string> GetRegionId(string raffleUrl, string region, CancellationToken ct)
        {
            var getRaffleHtml = await HttpClient.GetAsync(raffleUrl, ct);
            var raffleHtml = await getRaffleHtml.ReadStringResultOrFailAsync("Can't access raffle page", ct);
            
            var regexPattern = @"<option *value=""\d{1,3}"">"+region+@"<\/option>";
            var regexMatch = new Regex(regexPattern).Match(raffleHtml).ToString();
            
            var regionId = new Regex(@"value=""\d{1,3}""").Match(regexMatch).ToString().Replace("value=","").Replace(@"""","");

            return regionId;
        }

        public async Task<string> GetCaptchaImage(string raffleUrl, CancellationToken ct)
        {
            await HttpClient.GetAsync(raffleUrl, ct); //cookies
            
            var captchaUrl = "https://www.tike.ro/webservis/captcha/job.php?task=getcaptchaimage&form=all";

            var getCaptchaImage = await HttpClient.GetAsync(captchaUrl, ct);
            if (!getCaptchaImage.IsSuccessStatusCode)
                await getCaptchaImage.FailWithRootCauseAsync("Can't get captcha", ct);
            
            var captchaStream = await getCaptchaImage.Content.ReadAsByteArrayAsync(ct);
            
            var baseEncoded = Convert.ToBase64String(captchaStream);

            var captchaImageString = "data:image/jpeg;base64," + baseEncoded;

            return captchaImageString;
        }

        public async Task<bool> SubmitAsync(AddressFields address, string email, string raffleUrl, string captcha, string regionId, string houseNumber, string size, CancellationToken ct)
        {
            var street = address.AddressLine1.Value.Replace(houseNumber, "");
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"preorder_firstname", address.FirstName.Value},
                {"preorder_lastname", address.LastName.Value},
                {"preorder_email", email},
                {"preorder_phone", address.PhoneNumber.Value},
                {"preorder_region_id", regionId},
                {"preorder_city", address.City.Value},
                {"preorder_city_id", "-1"}, //figure out if needed, was always -1 for me
                {"preorder_postcode", address.PostCode.Value},
                {"preorder_address", street},
                {"preorder_address_id", ""}, //figure out if needed, is always null for me for some reason
                {"preorder_street_no", houseNumber},
                {"preorder_size", size},
                {"preorder_delivery", "1"},
                {"preorder_captchacode", captcha},
                {"preorder_terms_of_use","1"},
                {"preorder_submit","1"}
            });

            var post = await HttpClient.PostAsync(raffleUrl, content, ct);
            var postContent = await post.ReadStringResultOrFailAsync("Error on submitting", ct);

            if (postContent.Contains("Prijava je moguca samo sa teritorije Srbije"))
                await post.FailWithRootCauseAsync("Entry failed, use serbian address", ct);

            if (postContent.Contains("Campul anti-spam este incorect"))
                await post.FailWithRootCauseAsync("Captcha Error", ct);

            return post.IsSuccessStatusCode; //need to see what proper success message is
        }
    }
}