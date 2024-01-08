using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.WoodWoodModule
{
  public class WoodWoodSubmitPayload
  {
    public WoodWoodSubmitPayload(AddressFields address, Account account, string raffletag, string captcha, string ip, string phonecode)
    {
      Address = address;
      Account = account;
      ProductTag = raffletag;
      Captcha = captcha;
      Ip = ip;
      PhoneCode = phonecode;
    }
    public AddressFields Address { get; }
    public Account Account { get; }
    public string ProductTag { get; }
    public string Captcha { get; }
    public string Ip { get; }
    public string PhoneCode { get; }
  }
}