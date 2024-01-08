using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.NotreModule
{
    public class NotreClient : ModuleHttpClientBase, INotreClient
    {
        private readonly ICaptchaSolveService _captchaSolver;
        private readonly ICountriesService _countriesService;
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        public NotreClient(ICaptchaSolveService captchaSolver, ICountriesService countriesService)
        {
            _captchaSolver = captchaSolver;
            _countriesService = countriesService;
        }
        
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.CookieContainer = _cookieContainer;
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.193 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
                httpClient.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                httpClient.DefaultRequestHeaders.Add("accept-language", "en-US");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                httpClient.DefaultRequestHeaders.Add("sec-fetch-site","same-origin");
                httpClient.DefaultRequestHeaders.Add("sec-gpc", "1");
            };
        }

        public async Task<NotreParsed> ParseNotreAsync(CancellationToken ct)
        {
            await SetXsrf(ct);
            
            var raffleApi = "https://notreraffle.com/api/raffles?";
            var getRaffles = await HttpClient.GetAsync(raffleApi, ct);
            var raffleApiContent = await getRaffles.ReadStringResultOrFailAsync("Can't access raffles", ct);

            dynamic parseRafflesApi = JObject.Parse(raffleApiContent);
            var activeRaffles = parseRafflesApi.raffles;

            var raffleList = new List<int>();
            var raffleSizes = new Dictionary<int, List<string>>();
            
            foreach (var activeRaffle in activeRaffles)
            {
                int id = activeRaffle.id;
                raffleList.Add(id);
            }
            
            foreach (int raffleId in raffleList)
            {
                var activeRaffleApiUrl = "https://notreraffle.com/api/raffles/" + raffleId;
                var getRaffleApi = await HttpClient.GetAsync(activeRaffleApiUrl, ct);
                var activeRaffleApiContent =
                    await getRaffleApi.ReadStringResultOrFailAsync($"Can't get info for raffle {raffleId}", ct);

                dynamic parsedActiveRaffle = JObject.Parse(activeRaffleApiContent);
                
                int currentRaffleId = parsedActiveRaffle.raffle.id;
                dynamic currentRaffleSizeArray = (JArray)parsedActiveRaffle["raffle"]["items"][0]["allocations"];

                var currentRaffleSizes = new List<string>();
                foreach (var sizeObject in currentRaffleSizeArray)
                {
                    string size = sizeObject.size;
                    
                    currentRaffleSizes.Add(size);
                }
                
                raffleSizes.Add(currentRaffleId, currentRaffleSizes);
            }

            HttpClient.DefaultRequestHeaders.Remove("referer");
            HttpClient.DefaultRequestHeaders.Remove("x-xsrf-token");
            HttpClient.DefaultRequestHeaders.Remove("x-requested-with"); //remove it because it's getting set again afterwards, and it would error out
            
            return new NotreParsed(raffleList, raffleSizes);
        }
        
        
        public async Task SetXsrf(CancellationToken ct)
        {
            var uri = new Uri("https://notreraffle.com/raffles");
            
            var getCookies = await HttpClient.GetAsync("https://notreraffle.com/raffles", ct);
            var xsrfCookie = _cookieContainer.GetCookies(uri)["XSRF-TOKEN"]?.ToString().Replace("XSRF-TOKEN=","").Replace("%3D","=");
            if (string.IsNullOrEmpty(xsrfCookie))
                await getCookies.FailWithRootCauseAsync("Error getting XSRF cookie", ct);
            
            
            HttpClient.DefaultRequestHeaders.Add("x-requested-with","XMLHttpRequest");
            HttpClient.DefaultRequestHeaders.Add("x-xsrf-token", xsrfCookie);
        }

        public async Task<NotreQuestion> GetCaptchaQuestionAsync(CancellationToken ct)
        {
            HttpClient.DefaultRequestHeaders.Remove("referer");
            HttpClient.DefaultRequestHeaders.Add("referer", "https://notreraffle.com/register");
            
            //get captcha
            var getCaptcha = await HttpClient.GetAsync("https://notreraffle.com/api/question-answers/random", ct);
            var captchaResponse = await getCaptcha.ReadStringResultOrFailAsync("Couldn't get captcha question",ct);

            dynamic parseCaptcha = JObject.Parse(captchaResponse);

            string question = parseCaptcha.qna.question;

            var optionsList = new List<string>();

            var answerOptions = parseCaptcha.qna.options;
            foreach (var option in answerOptions)
            {
                string optionToAdd = option;
                optionsList.Add(optionToAdd);
            }

            string questionToken = parseCaptcha.qna.key;

            return new NotreQuestion(question, optionsList, questionToken, false, null);
        }

        public async Task<string> GetCaptchaAnswerAsync(string question, CancellationToken ct)
        {
            var recapv2 = await _captchaSolver.SolveReCaptchaV2Async("6LdhIq8UAAAAAO3N-yHHx5_vmutjCSQW47P4jLH1",
                "https://serpapi.com/", true, ct);
            
            var serpHttpClient = new HttpClient();
            
            var questionSubstringed = question.Substring(0, question.IndexOf("?", StringComparison.Ordinal) + 1);
            
            var url =
                $"https://serpapi.com/search.json?q={questionSubstringed}&hl=en&gl=us&async=false&gRecaptchaResponse={recapv2}";

            var getUrl = await serpHttpClient.GetAsync(url, ct);
            var urlAnswer = await getUrl.ReadStringResultOrFailAsync("Can't get answer", ct);
            
            dynamic results = JObject.Parse(urlAnswer);

            string answer = "";
            if (urlAnswer.Contains("sports_results"))
            {
                answer = results.sports_results.title; // this is almost never right - atleast it hasn't been when tested
            }

            if (urlAnswer.Contains("knowledge_graph"))
            {
                answer = results.knowledge_graph.title;
            }

            if (urlAnswer.Contains("answer_box"))
            {
                answer = results.answer_box.result;
                if(string.IsNullOrEmpty(answer)) answer = results.answer_box.answer;
            }

            return answer;
        }

        public Task<NotreQuestion> ValidateAnswerAsync(NotreQuestion question, string answer)
        {
            var validAnswer = "";
            var isValidAnswer = false;

            if (question.Question == "What team did the Chicago Bulls defeat in the 1992 NBA Finals?") //they love this question, and SERP can't find answer lmao
            {
                validAnswer = "Lakers";
                isValidAnswer = true;
            }

            if (question.Question == "In what US City is our store Notre located? (Answer is one word)") //same as above
            {
                validAnswer = "Chicago";
                isValidAnswer = true;
            }

            if (question.QuestionOptions.Contains(answer))
            {
                validAnswer = answer;
                isValidAnswer = true;
            }

            if (!question.QuestionOptions.Contains(answer))
            {
                if (question.QuestionOptions.Contains(answer, StringComparer.OrdinalIgnoreCase))
                {
                    var index = question.QuestionOptions.FindIndex(x => x.Equals(answer,StringComparison.OrdinalIgnoreCase));

                    validAnswer = question.QuestionOptions[index];
                    isValidAnswer = true;
                }

                /*
                if (askedQuestion.QuestionOptions.Any(l => l.Contains(serpAnswer)))
                {
                    validAnswer = askedQuestion.QuestionOptions.FirstOrDefault(s => s.Contains(serpAnswer));
                    isValidAnswer = true;
                } */

                if (!question.QuestionOptions.Contains(answer, StringComparer.OrdinalIgnoreCase))
                {
                    isValidAnswer = false;
                }
            }
            
            return Task.FromResult(new NotreQuestion(question.Question, question.QuestionOptions, question.QuestionToken, isValidAnswer, validAnswer));
        }

        public async Task RegisterUser(AddressFields addressFields, NotreQuestion question, string userEmail, CancellationToken ct)
        {
            var lookedUpState = _countriesService.GetProvinceName(addressFields.CountryId, addressFields.ProvinceId);
            
            var registerContent = new
            {
                state = lookedUpState,
                country = "United States",
                meta = new
                {
                    optins = new
                    {
                        
                    }
                },
                first_name = addressFields.FirstName.Value,
                last_name = addressFields.LastName.Value,
                email = userEmail,
                phone_number = addressFields.PhoneNumber.Value,
                street_address = addressFields.AddressLine1.Value,
                city = addressFields.City.Value,
                zip = addressFields.PostCode.Value,
                qna_key = question.QuestionToken,
                answer = question.Answer
            };
            var customer = JsonConvert.SerializeObject(registerContent);
            var content = new StringContent(customer, Encoding.UTF8, "application/json");

            var url = "https://notreraffle.com/api/register";
            var postContent = await HttpClient.PostAsync(url, content, ct);
            if (!postContent.IsSuccessStatusCode) await postContent.FailWithRootCauseAsync("Error on creating account", ct);
        }

        public async Task<bool> SubmitEntryToRaffleAsync(NotreParsed parsed, int raffle, string userSize,
            CancellationToken ct)
        {
            if (userSize.ToLower() == "random")
            {
                var rnd = new Random();
                parsed.RaffleIdSizes.TryGetValue(raffle, out List<string> sizeList);
                if(sizeList == null) throw new RaffleFailedException("No sizes found for raffle", "No sizes found for raffle");

                var randomSize = rnd.Next(sizeList.Count);
                userSize = sizeList[randomSize];
            }
            
            var endpoint = $"https://notreraffle.com/api/raffles/{raffle}/entry";
            var item = new
            {
                item_id = raffle,
                size = userSize
            };
            var entry = JsonConvert.SerializeObject(item);
            var content = new StringContent(entry, Encoding.UTF8, "application/json");
            
            var post2 = await HttpClient.PostAsync(endpoint, content, ct);
            if (!post2.IsSuccessStatusCode)
                await post2.FailWithRootCauseAsync($"Error on submission for raffle: {raffle}", ct);

            return post2.IsSuccessStatusCode;  
        }
        
        
    }
}