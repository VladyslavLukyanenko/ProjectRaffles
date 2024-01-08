using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.DelibertiModule
{
  public class DelibertiClient : ModuleHttpClientBase, IDelibertiClient
  {
    private readonly ICountriesService _countriesService;
    private readonly IBirthdayProviderService _birthdayProvider;

    public DelibertiClient(ICountriesService countriesService, IBirthdayProviderService birthdayProvider)
    {
      _birthdayProvider = birthdayProvider;
      _countriesService = countriesService;
    }
    
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
    

    public async Task<bool> SubmitAsync(AddressFields profile, string email, string size, string streetnumber, string region, string instagram, string raffleurl, CancellationToken ct)
    {
      var delibertiBase = await HttpClient.GetAsync(raffleurl, ct);
      if(!delibertiBase.IsSuccessStatusCode) await delibertiBase.FailWithRootCauseAsync("Can't access site", ct);
      
      var delibertiContentBase = await delibertiBase.Content.ReadAsStringAsync(ct);

      var country = _countriesService.GetCountryName(profile.CountryId)
        .Replace(" (" + profile.CountryId + ")", "");

      var countryRegexPattern = @"<option value=""\d{1,3}"">" + country + @"<\/option>";
      Regex countryRegex = new Regex(countryRegexPattern);
      var findCountryId = countryRegex.Match(delibertiContentBase).ToString();
      var countryId = findCountryId.Replace(@"<option value=""", "").Replace(country, "")
        .Replace(@"""></option>", "");

      var birthDay = await _birthdayProvider.GetDate();
      var birthMonth = await _birthdayProvider.GetMonth();
      var birthYear = await _birthdayProvider.GetYear();
      
      var dob = $"{birthDay}/{birthMonth}/{birthYear}";

      var streetAddress = profile.AddressLine1.ToString().Replace(streetnumber, "");

      var textarea =
        @"Deliberti Service srl garantisce che ogni informazione fornita a fini promozionali verrà trattata in conformità al Decreto Lgs. 196/2003.
Deliberti Service srl comunica inoltre che ai sensi del Decreto Lgs. 196/2003 i dati degli utenti forniti al momento della sottoscrizione dell'ordine di acquisto e/o della compilazione della fattura, sono esclusi dal consenso dell'interessato in quanto raccolti in base agli obblighi fiscali/tributari previsti dalla legge, dai regolamenti e dalla normativa comunitaria e, in ogni caso, al solo fine di adempiere agli obblighi derivanti dal contratto di acquisto cui è parte interessato e/o per l'acquisizione delle necessarie informative contrattuali sempre ed esclusivamente attivate su richiesta di quest'ultimo (Art. 24, Lett. A e B, D. LGS. 196/2003).
In particolare Deliberti Service srl precisa che i dati personali forniti dai propri Clienti non saranno utilizzati ai fini di informazione commerciale e/o di invio di materiale pubblicitario ovvero per il compimento di ricerche di mercato o di comunicazione commerciale interattiva, se non a seguito della preventivo consenso da parte del Cliente.
I dati sono trattati elettronicamente nel rispetto delle leggi vigenti e potranno essere esibiti soltanto su richiesta della autorità giudiziaria ovvero di altre autorità all'uopo autorizzate dalla legge.
L'interessato gode dei diritti di cui all'art. 7 Decreto Lgs. 196/2003, e cioè: di chiedere conferma della esistenza presso la sede della Deliberti Service srl dei propri dati personali; di conoscere la loro origine, la logica e le finalità del loro trattamento; di ottenere l'aggiornamento, la rettifica, e la integrazione; di chiederne la cancellazione, la trasformazione in forma anonima o il blocco in caso di trattamento illecito; di opporsi al loro trattamento per motivi legittimi o nel caso di utilizzo dei dati per invio di materiale pubblicitario, informazioni commerciali, ricerche di mercato, di vendita diretta e di comunicazione commerciale interattiva.
L'ottenimento della cancellazione dei propri dati personali e' subordinato all'invio di una comunicazione scritta inviata tramite mail (privacy@deliberti.it o spedizione postale alla sede della società. Titolare alla raccolta dei dati personali è Deliberti Service srl, Via J.F. Kennedy, 5 - 80125 Napoli, nella persona del suo legale rappresentante.";
      
      var content = new FormUrlEncodedContent( new Dictionary<string,string>
      {
        {"action", "process"},
        {"firstname", profile.FirstName},
        {"lastname", profile.LastName},
        {"email_address", email},
        {"street_address", streetAddress},
        {"nr_address", streetnumber},
        {"country", countryId},
        {"city", profile.City},
        {"state", region},
        {"postcode", profile.PostCode},
        {"cellulare", profile.PhoneNumber},
        {"datanascita", dob},
        {"instagram", instagram},
        {"taglia", size},
        {"textarea", textarea}
      });
      
      var endpoint = "https://deliberti.it/raffle.php";
      
      var resp1 = await HttpClient.PostAsync(endpoint, content, ct);
      if(!resp1.IsSuccessStatusCode) await delibertiBase.FailWithRootCauseAsync("Can't submit entry", ct);
      
      var respHtml = await resp1.Content.ReadAsStringAsync(ct);

      return respHtml.Contains("Grazie");
    }
  }
}