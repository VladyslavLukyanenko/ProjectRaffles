namespace ProjectIndustries.ProjectRaffles.Core.Domain.Emails
{
  public interface ICatchAllEmailMaterializer
  {
    string Materialize(string catchallEmail);
  }
}