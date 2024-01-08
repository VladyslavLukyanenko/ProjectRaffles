// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Net.Http;
// using System.Threading;
// using System.Threading.Tasks;
// using HtmlAgilityPack;
// using ProjectIndustries.ProjectRaffles.Core.Domain;
// using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
// using ProjectIndustries.ProjectRaffles.Core.Services;
// using ProjectIndustries.ProjectRaffles.Core.Services.Captchas;
//
// namespace ProjectIndustries.ProjectRaffles.Core.Modules.EvgaModule
// {
//   public class EvgaAccountGenerator : IAccountGenerator
//   {
//     private readonly HttpClient _httpClient;
//     private readonly IPasswordGenerator _passwordGenerator;
//     private readonly IUsernameProviderService _usernameGenerator;
//     private readonly ICaptchaSolveService _captchaSolver;
//
//     private readonly string _accountRegisterUrl = "https://secure.evga.com/US/signup.asp";
//
//     public EvgaAccountGenerator(IPasswordGenerator passwordGenerator, IUsernameProviderService usernameGenerator,
//       ICaptchaSolveService captchaSolver)
//     {
//       _passwordGenerator = passwordGenerator;
//       _usernameGenerator = usernameGenerator;
//       _captchaSolver = captchaSolver;
//
//       var httpClientHandler = new HttpClientHandler()
//       {
//         AllowAutoRedirect = false,
//         CookieContainer = new CookieContainer(),
//         UseCookies = true
//       };
//
//       _httpClient = new HttpClient(httpClientHandler);
//
//       _httpClient.DefaultRequestHeaders.Add("User-Agent",
//         "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
//       _httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
//       _httpClient.DefaultRequestHeaders.Add("dnt", "1");
//     }
//
//     public async Task<string> ParseFormAsync(CancellationToken ct)
//     {
//       var getBody = await _httpClient.GetAsync(_accountRegisterUrl, ct);
//       var body = await getBody.Content.ReadAsStringAsync();
//
//       var doc = new HtmlDocument();
//       doc.LoadHtml(body);
//
//       var ncForm = doc.DocumentNode.SelectSingleNode("//input[@name='__ncforminfo']")
//         .GetAttributeValue("value", "");
//
//       return ncForm;
//     }
//
//     public async Task<Account> SubmitAsync(Profile profile, string ncForm, string username, string captcha,
//       CancellationToken ct)
//     {
//       var password = _passwordGenerator.Generate(12);
//
//       var content = new FormUrlEncodedContent(new Dictionary<string, string>
//       {
//         {"country", "US"},
//         {"fname", profile.ShippingAddress.FirstName},
//         {"lname", profile.ShippingAddress.LastName},
//         {"addr1", profile.ShippingAddress.AddressLine1},
//         {"addr2", profile.ShippingAddress.AddressLine2},
//         {"city", profile.ShippingAddress.City},
//         {"state", profile.ShippingAddress.ProvinceCode},
//         {"state1", ""},
//         {"province", ""},
//         {"province1", ""},
//         {"zip", profile.ShippingAddress.ZipCode},
//         {"phone", profile.ShippingAddress.PhoneNumber},
//         {"email", profile.Email},
//         {"cEmail", profile.Email},
//         {"login", username},
//         {"password", password},
//         {"cPassword", password},
//         {"agree1", "1"},
//         {"agree2", "1"},
//         {"mailgeneral", "1"},
//         {"languageoption", "US/Canada – English"},
//         {"g-recaptcha-response", captcha},
//         {"qrsn", ""},
//         {"qremail", ""},
//         {"action", "process"},
//         {"__ncforminfo", ncForm}
//       });
//
//       var finalizeRegistration = await _httpClient.PostAsync(_accountRegisterUrl, content, ct);
//       var finalBody = await finalizeRegistration.Content.ReadAsStringAsync();
//
//       if (finalBody.Contains("Thank you for your product alert subscription"))
//       {
//         return new Account(profile.Email, password);
//       }
//
//       throw new InvalidOperationException("Creation");
//     }
//
//     public async Task<Account> GenerateAsync(Profile profile, CancellationToken ct = default)
//     {
//       var formInfoKey = await ParseFormAsync(ct);
//
//       var username = await _usernameGenerator.GenerateUsernameAsync(ct);
//
//       var captcha =
//         await _captchaSolver.SolveReCaptchaV2Async("6Lf22-sSAAAAACayYWhxiHN_554ipXEU4bjwQjY7", _accountRegisterUrl,
//           true, ct);
//
//       var createAccount = await SubmitAsync(profile, formInfoKey, username, captcha, ct);
//
//       return createAccount;
//     }
//   }
// }