namespace ProjectIndustries.ProjectRaffles.Core.Modules.VoostoreBerlinModule
{
  public class VoostoreBerlinRaffleTags
  {
    public VoostoreBerlinRaffleTags(string token, string pageid)
    {
      Token = token;
      PageId = pageid;
    }

    public string Token { get; }
    public string PageId { get; }
  }
}