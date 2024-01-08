namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
    public class ShortTextFormField : TextTypeFormFieldBase
    {
        public ShortTextFormField(string id, string label)
            : base("text", TypeFormFieldDescriptor.ShortText(id), label)
        {
        }
    }
}