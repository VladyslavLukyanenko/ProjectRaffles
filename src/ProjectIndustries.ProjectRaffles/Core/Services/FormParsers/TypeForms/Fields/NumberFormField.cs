namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
    public class NumberFormField : TextTypeFormFieldBase
    {
        public NumberFormField(string id, string label)
            : base("number", TypeFormFieldDescriptor.Number(id), label)
        {
        }
    }
}