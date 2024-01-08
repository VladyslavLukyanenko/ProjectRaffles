using System;
using System.Runtime.Serialization;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  [DataContract]
  public class GeneralSettings
  {
    private readonly Random _rnd = new Random((int) DateTime.Now.Ticks);

    [DataMember] public TimeSpan MinimumDelay { get; set; }
    [DataMember] public TimeSpan MaximumDelay { get; set; }
    [DataMember] public string CatchAllEmailMaterializeTemplate { get; set; } = "%Guid%";

    public TimeSpan GenerateDelay()
    {
      return TimeSpan.FromMilliseconds(_rnd.Next((int) MinimumDelay.TotalMilliseconds,
        (int) MaximumDelay.TotalMilliseconds));
    }
  }
}