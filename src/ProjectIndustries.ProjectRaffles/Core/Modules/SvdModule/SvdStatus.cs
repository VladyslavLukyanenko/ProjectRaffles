using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public static class SvdStatus
  {
    public static RaffleStatus FetchingShippingMethod { get; } =
      new RaffleStatus("Fetching Shipping Method", RaffleStatusKind.InProgress);

    public static RaffleStatus ObtainingAuthorizationFingerprint { get; } =
      new RaffleStatus("Authorizing", RaffleStatusKind.InProgress);

    public static RaffleStatus Demo(string text)=>
      new RaffleStatus(nameof(Demo), RaffleStatusKind.InProgress, text);
    
    public static RaffleStatus Submitting { get; } =
      new RaffleStatus(nameof(Submitting), RaffleStatusKind.InProgress);
  }
}