namespace ProjectIndustries.ProjectRaffles.Core.Modules.StayRootedModule
{
  public class StayRootedFinalPayload
  {
    public StayRootedFinalPayload(string checkouttoken, string entryid)
    {
      PaymentToken = checkouttoken;
      EntryId = entryid;
    }

    public string PaymentToken { get; }
    public string EntryId { get; }
  }
}