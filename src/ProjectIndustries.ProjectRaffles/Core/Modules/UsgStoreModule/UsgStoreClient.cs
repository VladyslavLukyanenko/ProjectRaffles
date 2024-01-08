using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.UsgStoreModule
{
    public class UsgStoreClient : ModuleHttpClientBase, IUsgStoreClient
    {
        protected override void ConfigureHttpClient(HttpClientOptions options)
        {
            options.PostConfigure = httpClient =>
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                httpClient.DefaultRequestHeaders.Add("dnt", "1");
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
        
        public async Task<UsgStoreParsed> ParseRaffleAsync(string raffleUrl, CancellationToken ct)
        {
            var getRafflePage = await HttpClient.GetAsync(raffleUrl, ct);
            var rafflePageBody = await getRafflePage.ReadStringResultOrFailAsync("Can't access site", ct);

            var mailmunchFormRegexPattern = @"mailmunch-forms-widget-\d*";
            var mailmunchRegex = new Regex(mailmunchFormRegexPattern);
            var mailmunchId = mailmunchRegex.Match(rafflePageBody).ToString().Replace("mailmunch-forms-widget-", "");
            var mailMunchPage = $"https://a.mailmunch.co/forms-cache/788965/{mailmunchId}/index.html";
            
            var getMailMunchPage = await HttpClient.GetAsync(mailMunchPage, ct);
            var mailmunchBody = await getMailMunchPage.ReadStringResultOrFailAsync("Can't access form", ct);
            
            var doc = new HtmlDocument();
            doc.LoadHtml(mailmunchBody);
            
            var firstNameId = doc.DocumentNode.SelectSingleNode($"//input[@value='FIRST NAME']")
                .GetAttributeValue("name", "").Replace("contact[contact_fields_attributes][", "")
                .Replace(@"][label]", "");
            
            var lastNameId = doc.DocumentNode.SelectSingleNode($"//input[@value='LAST NAME']")
                .GetAttributeValue("name", "").Replace("contact[contact_fields_attributes][", "")
                .Replace(@"][label]", "");
            
            var emailId = doc.DocumentNode.SelectSingleNode($"//input[@value='EMAIL']")
                .GetAttributeValue("name", "").Replace("contact[contact_fields_attributes][", "")
                .Replace(@"][label]", "");
            
            
            var phoneNumberId = doc.DocumentNode.SelectSingleNode($"//input[@value='PHONE NUMBER']")
                .GetAttributeValue("name", "").Replace("contact[contact_fields_attributes][", "")
                .Replace(@"][label]", "");
            
            var addressId = doc.DocumentNode.SelectSingleNode($"//input[@value='ADDRESS']")
                .GetAttributeValue("name", "").Replace("contact[contact_fields_attributes][", "")
                .Replace(@"][label]", "");

            /*
            var sizeRegexPattern = @"custom_field_dropdown_\d*"">SIZE";
            var sizeRegex = new Regex(sizeRegexPattern);
            var sizeMatch = sizeRegex.Match(mailmunchBody).ToString();
            var sizeId = sizeMatch.Replace(@"custom_field_dropdown_", "").Replace(@""">SIZE", ""); */
            
            var sizeId = doc.DocumentNode.SelectSingleNode($"//input[@value='SIZE']")
                .GetAttributeValue("name", "").Replace("contact[contact_fields_attributes][", "")
                .Replace(@"][label]", "");

            var firstNameFieldId = doc.DocumentNode.SelectSingleNode($"//input[@name='contact[contact_fields_attributes][{firstNameId}][custom_field_id]']").GetAttributeValue("value", "");
            
            var lastNameFieldId = doc.DocumentNode.SelectSingleNode($"//input[@name='contact[contact_fields_attributes][{lastNameId}][custom_field_id]']").GetAttributeValue("value", "");

            var sizeFieldId = doc.DocumentNode.SelectSingleNode($"//input[@name='contact[contact_fields_attributes][{sizeId}][custom_field_id]']").GetAttributeValue("value", "");

            var emailFieldId = doc.DocumentNode.SelectSingleNode($"//input[@name='contact[contact_fields_attributes][{emailId}][custom_field_id]']").GetAttributeValue("value", "");
            
            var phoneNumberFieldId = doc.DocumentNode.SelectSingleNode($"//input[@name='contact[contact_fields_attributes][{phoneNumberId}][custom_field_id]']").GetAttributeValue("value", "");

            var addressFieldId = doc.DocumentNode.SelectSingleNode($"//input[@name='contact[contact_fields_attributes][{addressId}][custom_field_id]']").GetAttributeValue("value", "");

            return new UsgStoreParsed(firstNameId, firstNameFieldId, lastNameId, lastNameFieldId, sizeId, sizeFieldId, emailId, emailFieldId, phoneNumberId, phoneNumberFieldId, addressId, addressFieldId, mailmunchId, mailMunchPage);
        }

        public async Task<bool> SubmitAsync(AddressFields profile, string email, UsgStoreParsed parsed, string size, string raffleurl, string captcha, CancellationToken ct)
        {
            var address = profile.AddressLine1.Value + ", " + profile.PostCode.Value + ", " + profile.City.Value + ", " +
                          profile.ProvinceId.Value;
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {$"contact[contact_fields_attributes][{parsed.FirstNameId}][value]", profile.FirstName.Value},
                {$"contact[contact_fields_attributes][{parsed.FirstNameId}][label]", "FIRST NAME"},
                {$"contact[contact_fields_attributes][{parsed.FirstNameId}][custom_field_data_id]", parsed.FirstNameId},
                {$"contact[contact_fields_attributes][{parsed.FirstNameId}][custom_field_id]", parsed.FirstNameFieldId},

                {$"contact[contact_fields_attributes][{parsed.LastNameId}][value]", profile.LastName.Value},
                {$"contact[contact_fields_attributes][{parsed.LastNameId}][label]", "LAST NAME"},
                {$"contact[contact_fields_attributes][{parsed.LastNameId}][custom_field_data_id]", parsed.LastNameId},
                {$"contact[contact_fields_attributes][{parsed.LastNameId}][custom_field_id]", parsed.LastNameFieldId},

                {$"contact[contact_fields_attributes][{parsed.SizeId}][value]", size},
                {$"contact[contact_fields_attributes][{parsed.SizeId}][label]", "SIZE"},
                {$"contact[contact_fields_attributes][{parsed.SizeId}][custom_field_data_id]", parsed.SizeId},
                {$"contact[contact_fields_attributes][{parsed.SizeId}][custom_field_id]", parsed.SizeFieldId},
                
                {$"contact[contact_fields_attributes][{parsed.EmailId}][value]", email},
                {$"contact[contact_fields_attributes][{parsed.EmailId}][label]", "EMAIL"},
                {$"contact[contact_fields_attributes][{parsed.EmailId}][custom_field_data_id]", parsed.EmailId},
                {$"contact[contact_fields_attributes][{parsed.EmailId}][custom_field_id]", parsed.EmailFieldId},
                
                {$"contact[contact_fields_attributes][{parsed.PhoneNumberId}][value]", profile.PhoneNumber.Value},
                {$"contact[contact_fields_attributes][{parsed.PhoneNumberId}][label]", "PHONE NUMBER"},
                {$"contact[contact_fields_attributes][{parsed.PhoneNumberId}][custom_field_data_id]", parsed.PhoneNumberId},
                {$"contact[contact_fields_attributes][{parsed.PhoneNumberId}][custom_field_id]", parsed.PhoneNumberFieldId},
                
                {$"contact[contact_fields_attributes][{parsed.AddressId}][value]", address},
                {$"contact[contact_fields_attributes][{parsed.AddressId}][label]", "ADDRESS"},
                {$"contact[contact_fields_attributes][{parsed.AddressId}][custom_field_data_id]", parsed.AddressId},
                {$"contact[contact_fields_attributes][{parsed.AddressId}][custom_field_id]", parsed.AddressFieldId},
                
                {"referrer", raffleurl},
                {"recaptcha_token", captcha},
                {"email_address", email},
                {"visitor_id", Guid.NewGuid().ToString()},
            });

            var endpoint = $"https://forms.mailmunch.co/form/788965/{parsed.FormId}/submit";

            var postEntry = await HttpClient.PostAsync(endpoint, content, ct);
            if (!postEntry.IsSuccessStatusCode) await postEntry.FailWithRootCauseAsync("Entry failed", ct);

            return postEntry.IsSuccessStatusCode;
        }
    }
}