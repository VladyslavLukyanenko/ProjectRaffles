using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PaymentProcessors
{
    public class BraintreeProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly string GRAPHQL_URL = "https://payments.braintree-api.com/graphql";
        public BraintreeProcessor(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent","braintree android 3.11.1");
            _httpClient.DefaultRequestHeaders.Add("Braintree-Version", "2018-03-06");
        }

        public async Task<string> Parse(string authorizationFingerprint, string merchantId, Profile profile)
        {

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authorizationFingerprint}");
            string sessionId = Guid.NewGuid().ToString("N");
            GraphQL graphql = new GraphQL
            {
                clientsdkmetadata = new ClientSdkMetadata()
                {
                    integration = "custom",
                    platform = "android",
                    sessionId = sessionId,
                    source = "form"
                },
                operationName = "TokenizeCreditCard",
                query =
                    "mutation TokenizeCreditCard($input: TokenizeCreditCardInput!) {  tokenizeCreditCard(input: $input) {    token    creditCard {      bin      brand      last4      binData {        prepaid        healthcare        debit        durbinRegulated        commercial        payroll        issuingBank        countryOfIssuance        productId      }    }  }}",
                variables = new Variables()
                {
                    input = new Input()
                    {
                        creditCard = profile.CreditCard,
                        options = new Options()
                        {
                            validate = true
                        }
                    }
                }
            };

            var graphqlData = JsonConvert.SerializeObject(graphql);
            var graphqldata = new StringContent(graphqlData, Encoding.UTF8, "application/json");
            var graphQlResponse = await _httpClient.PostAsync(GRAPHQL_URL, graphqldata);
            string graphqlresult = await graphQlResponse.Content.ReadAsStringAsync();
            dynamic graphqltokenized = JObject.Parse(graphqlresult);
            string graphqltoken = graphqltokenized.data.tokenizeCreditCard.token;

            var braintreegatewayurl =
                $"https://api.braintreegateway.com/merchants/{merchantId}/client_api/v1/payment_methods/{graphqltoken}/three_d_secure/lookup";
            BraintreeGateway braintreegateway = new BraintreeGateway
            {
                additional_info = new additionalinfo(),
                amount = "", //raffle price amount
                authorizationFingerprint = authorizationFingerprint,
                challenge_requested = false,
                exemption_requested = false,
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "braintree android 3.11.1");

            var braintreeGateway = JsonConvert.SerializeObject(braintreegateway);
            var braintreegatewaydata = new StringContent(braintreeGateway, Encoding.UTF8, "application/json");
            var gatewayresult = await _httpClient.PostAsync(braintreegatewayurl, braintreegatewaydata);
            string gatewayresultstring = await gatewayresult.Content.ReadAsStringAsync();
            dynamic gatewayparse = JObject.Parse(gatewayresultstring);
            string pareq = gatewayparse.lookup.pareq;
            string md = gatewayparse.lookup.md;
            string paymentmethod = gatewayparse.paymentMethod.nonce;
            string termurl = gatewayparse.lookup.termUrl;
            string acsurl = gatewayparse.lookup.acsUrl;

            string termEscape = Uri.EscapeDataString(termurl);
            string acsEscape = Uri.EscapeDataString(acsurl);

            string redirecturl =
                $"https://assets.braintreegateway.com/mobile/three-d-secure-redirect/0.2.0/index.html?AcsUrl={acsEscape}&PaReq={pareq}&MD={md}&TermUrl={termEscape}&ReturnUrl=https%3A%2F%2Fassets.braintreegateway.com%2Fmobile%2Fthree-d-secure-redirect%2F0.2.0%2Fredirect.html%3Fredirect_url%253Dcom.sivasdescalzo.svdapp.braintree%25253A%25252F%25252Fx-callback-url%25252Fbraintree%25252Fthreedsecure%25253F";

            throw new NotImplementedException();
        }
    }
}