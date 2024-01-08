namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
    public class LongTextFormField : TextTypeFormFieldBase
    {
        public LongTextFormField(string id, string label)
            : base("text", TypeFormFieldDescriptor.LongText(id), label)
        {
        }
    }
}