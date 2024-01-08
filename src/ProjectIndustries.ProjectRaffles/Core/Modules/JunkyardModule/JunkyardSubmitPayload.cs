using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.JunkyardModule
{
  public class JunkyardSubmitPayload
  {
    public JunkyardSubmitPayload(string email, string captcha, string raffleurl, JunkyardParsedRaffleFields parsedraffle)
    {
      Email = email;
      RaffleUrl = raffleurl;
      Captcha = captcha;
      ParsedRaffle = parsedraffle;
    }

    public string Email { get; private set; }
    public string RaffleUrl { get; private set;}
    public string SizeValue { get; private set;}
    public string Captcha { get; private set;}
    public JunkyardParsedRaffleFields ParsedRaffle { get; private set; }
  }
}