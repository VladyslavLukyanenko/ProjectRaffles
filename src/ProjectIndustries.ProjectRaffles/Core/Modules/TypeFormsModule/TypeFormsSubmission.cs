namespace ProjectIndustries.ProjectRaffles.Core.Modules.TypeFormsModule
{
  public class TypeFormsSubmission
  {
    public TypeFormsSubmission(string signature, int landedat, string formId)
    {
      Signature = signature;
      LandedAt = landedat;
      FormId = formId;
    }

    public string Signature { get; }
    public int LandedAt { get; }
    public string FormId { get; }
  }
}