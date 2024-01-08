namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
    public class WebsiteFormField : TextTypeFormFieldBase
    {
        public WebsiteFormField(string id, string label)
            : base("url", TypeFormFieldDescriptor.Website(id), label)
        {
        }
    }
}