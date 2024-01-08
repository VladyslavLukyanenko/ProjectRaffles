namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class TextTypeFormField : TextTypeFormFieldBase
  {
    public TextTypeFormField(string id, string label)
      : base("text", TypeFormFieldDescriptor.Text(id), label)
    {
    }
  }
}