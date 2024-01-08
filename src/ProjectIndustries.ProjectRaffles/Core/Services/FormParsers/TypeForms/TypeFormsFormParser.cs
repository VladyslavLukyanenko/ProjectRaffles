using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms
{
  public class TypeFormsFormParser : IFormParser
  {
    private HttpClient _httpClient;
    public bool IsModuleSupported(RaffleModuleType moduleType) => moduleType == RaffleModuleType.TypeForms;

    public TypeFormsFormParser()
    {
      _httpClient = new HttpClient();
    }


    public async ValueTask<FormParseResult> ParseAsync(Uri url, CancellationToken ct = default)
    {
      var getIdRegex = new Regex(@"https:\/\/.*\.typeform\.com\/to\/");
      var rawUrl = url.ToString();
      var removeTypeformUrlMatch = getIdRegex.Match(rawUrl).ToString();
      var removeTypeformUrl = rawUrl.Replace(removeTypeformUrlMatch, "");

      var createApiUrl = "https://api.typeform.com/forms/" + removeTypeformUrl;
      var getApi = await _httpClient.GetAsync(createApiUrl, ct);
      var apiContent = await getApi.Content.ReadAsStringAsync(ct);

      dynamic jObject = JObject.Parse(apiContent);

      string id = jObject.id;
      string title = jObject.title;

      var fieldsJson = jObject.fields;

      var fields = new List<TypeFormField>();
      foreach (var rawField in fieldsJson)
        //todo: add if field is required, can be checked with: field.validations.required
      {
        string fieldType = rawField.type;
        var field = GetFieldType(fieldType, rawField);
        if(field != null) fields.Add(field);
      }

      return new TypeFormParseResult(title, url, fields, apiContent);
    }

    private TypeFormField GetFieldType(string type, dynamic rawField)
    {
      string fieldId = rawField.id;
      string title = rawField.title;
      switch (type)
      {
        case "multiple_choice":
        {
          var options = new List<MultipleChoicesOption>();
          var choices = rawField.properties.choices;

          foreach (var choice in choices)
          {
            string id = choice.id;
            string label = choice.label;
            options.Add(new MultipleChoicesOption(id, label));
          }

          return new MultipleChoicesTypeFormField(fieldId, title, options);
        }

        case "dropdown":
        {
          var options = new List<string>();
          var choices = rawField.properties.choices;

          foreach (var choice in choices)
          {
            string id = choice.id;
            string label = choice.label;
            options.Add(label);
          }

          return new DropdownTypeFormField(fieldId, title, options);
        }

        case "short_text":
        {
          return new ShortTextFormField(fieldId, title);
        }

        case "long_text":
        {
          return new LongTextFormField(fieldId, title);
        }

        case "rating":
        {
          return new RatingFormField(fieldId, title);
        }

        case "url":
        {
          return new WebsiteFormField(fieldId, title);
        }

        case "number":
        {
          return new NumberFormField(fieldId, title);
        }

        case "opinion_scale":
        {
          return new OpinionScaleFormField(fieldId, title, (int)rawField.properties.steps,
            (bool) rawField.properties.start_at_one);
        }

        case "yes_no":
        {
          return new YesNoFormField(fieldId, title);
        }

        case "picture_choice":
        {
          var options = new List<MultipleChoicesOption>();
          var choices = rawField.properties.choices;

          foreach (var choice in choices)
          {
            string id = choice.id;
            string label = choice.label;
            options.Add(new MultipleChoicesOption(id, label));
          }
          
          return new MultipleChoicesTypeFormField(fieldId, title, options);
        }
        case "legal":
        {
          return new LegalFormField(fieldId, title);
        }
        case "phone_number":
        {
          return new PhoneNumberFormField(fieldId, title);
        }
        case "email":
        {
          return new EmailFormField(fieldId, title);
        }
        case "statement":
        {
          return null;
        }
        default: return new TextTypeFormField(fieldId, title);
      }
    }
  }
}