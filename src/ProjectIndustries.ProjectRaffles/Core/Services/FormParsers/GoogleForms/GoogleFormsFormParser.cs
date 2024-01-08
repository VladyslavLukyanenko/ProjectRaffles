using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.FieldMappers;
using ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms.Models;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.GoogleForms
{
  public class GoogleFormsFormParser : IFormParser
  {
    private readonly HttpClient _httpClient;
    private readonly IList<IGoogleFormFieldMapper> _fieldMappers;

    public GoogleFormsFormParser(IList<IGoogleFormFieldMapper> fieldMappers)
    {
      _fieldMappers = fieldMappers;
      _httpClient = new HttpClient();
    }

    string pattern = @"var FB_PUBLIC_LOAD_DATA_ = ([^;]*);<\/script>";

    private static readonly Regex FormIdMatchRegex = new Regex(@"https:\/\/docs\.google\.com\/forms\/d\/e\/([^\/]*)",
      RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public bool IsModuleSupported(RaffleModuleType moduleType) => moduleType == RaffleModuleType.GoogleForms;

    public async ValueTask<FormParseResult> ParseAsync(Uri url, CancellationToken ct = default)
    {
      var (json, rawResponse, reCaptchaKey) = await FetchFormJsonAsync(url, ct);
      var res = ParseForm(json);

      var fields = MapFields(res);
      return new GoogleFormParseResult(res.Form.Title, url, fields, rawResponse, reCaptchaKey);
    }

    private async ValueTask<(dynamic Result, string RawResponse, string ReCaptchaKey)> FetchFormJsonAsync(Uri url,
      CancellationToken ct)
    {
      var response = await _httpClient.GetAsync(url, ct);
      var body = await response.Content.ReadAsStringAsync(ct);
      if (response.StatusCode == HttpStatusCode.NotFound)
      {
        throw new Exception("Form not found");
      }

      if (body.Contains("PasswordSeparationSignIn"))
      {
        throw new Exception("Login required || Error");
      }

      var idMatch = FormIdMatchRegex.Match(url.PathAndQuery);
      var id = idMatch.Groups[1].Value;

      if (body.Contains("docs-crp\":\"/forms/d/e/" + id + "/closedform"))
      {
        throw new Exception("Form is closed");
      }

      var match = Regex.Matches(body, pattern).FirstOrDefault()?.Groups[1].Value;
      dynamic json = JsonConvert.DeserializeObject<JArray>(match!);
      json = json[1];
      if (json == null)
      {
        throw new Exception("Can't parse JSON");
      }

      var context = BrowsingContext.New();
      var doc = await context.OpenAsync(_ => _.Content(body), ct);
      var captcaEl = doc.GetElementById("recaptcha");
      string sitekey = null;
      if (captcaEl != null)
      {
        sitekey = captcaEl.Attributes["data-sitekey"].Value;
      }

      return (json, body, sitekey);
    }

    private Response ParseForm(dynamic json)
    {
      var res = new Response {Success = true, Form = new Form()};
      res.Form.Title = json.Root[3].ToString();
      res.Form.Description = json[0].ToString();
      res.Form.Login = ((JArray) json).Count >= 19 && json[18] != null;
      var requiresEmail = json[10] is not JValue && ((JValue) json[10][4]).Value<int>() == 1;
      if (requiresEmail)
      {
        res.Form.Fields.Add(new GoogleFormField
        {
          Required = true,
          Type = GoogleFormFieldType.EmailField,
          Id = "emailAddress",
          Name = "Email address"
        });
      }

      var formJson = json[1];
      if (formJson is not JArray)
      {
        return res;
      }

      foreach (var field in formJson)
      {
        var f = new GoogleFormField
        {
          Id = "entry." + field[0]?.ToString(),
          Name = field[1]?.ToString(),
          Type = Enum.Parse<GoogleFormFieldType>(field[3]?.ToString()!),
        };

        JArray ar = field;
        if (ar.Count >= 5)
        {
          if (field[4] is JArray)
          {
            f.Id = "entry." + field[4][0][0].ToString();
            JValue v = field[4][0][2];
            f.Required = v.Value<int>() == 1;
            f.Options = GetOptions(field[4][0][1]);
          }
        }

        res.Form.Fields.Add(f);
      }

      return res;
    }

    private IList<Field> MapFields(Response res)
    {
      var fields = new List<Field>();
      foreach (var googleFormField in res.Form.Fields)
      {
        var mapper = _fieldMappers.FirstOrDefault(_ => _.IsFieldSupported(googleFormField.Type));
        // if (mapper == null)
        // {
        // throw new InvalidOperationException("GoogleFormField mapper not found for field type "
        // + googleFormField.Type);
        // }

        var mappedFields = mapper?.Map(googleFormField) ?? FallbackCreateField(googleFormField);
        fields.AddRange(mappedFields);
      }

      return fields;
    }

    private IEnumerable<Field> FallbackCreateField(GoogleFormField googleFormField)
    {
      yield return new TextField(googleFormField.Id, googleFormField.Name, googleFormField.Required);
    }

    List<Option> GetOptions(dynamic options)
    {
      if (options == null)
      {
        return null;
      }

      var parsedOptions = new List<Option>();
      foreach (var item in options)
      {
        JArray arr = item;
        if (arr == null)
        {
          continue;
        }

        var res = new Option();
        res.Name = arr[0].Value<string>();
        if (arr.Count >= 5)
        {
          res.Custom = arr[4].Value<int>() == 1;
        }

        parsedOptions.Add(res);
      }

      return parsedOptions;
    }
  }
}