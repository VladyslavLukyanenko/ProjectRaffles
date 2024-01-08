namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
    public class RatingFormField : TextTypeFormFieldBase
    {
        public RatingFormField(string id, string label)
            : base("number", TypeFormFieldDescriptor.Rating(id), label)
        {
        }
    }
}