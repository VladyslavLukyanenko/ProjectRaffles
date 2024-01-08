namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class MultipleChoicesOption
  {
    public MultipleChoicesOption(string id, string label)
    {
      Id = id;
      Label = label;
    }

    public string Id { get; }
    public string Label { get; }
  }
}