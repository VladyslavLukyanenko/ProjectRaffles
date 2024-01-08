namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.TypeForms.Fields
{
  public class TypeFormFieldDescriptor
  {
    private TypeFormFieldDescriptor(string id, string type)
    {
      Id = id;
      Type = type;
    }

    public string Id { get; }
    public string Type { get; }

    public static TypeFormFieldDescriptor Text(string id) => new TypeFormFieldDescriptor(id, "text");
    public static TypeFormFieldDescriptor PhoneNumber(string id) => new TypeFormFieldDescriptor(id, "phone_number");
    public static TypeFormFieldDescriptor ShortText(string id) => new TypeFormFieldDescriptor(id, "short_text");
    public static TypeFormFieldDescriptor LongText(string id) => new TypeFormFieldDescriptor(id, "long_text");
    public static TypeFormFieldDescriptor YesNo(string id) => new TypeFormFieldDescriptor(id, "yes_no");
    public static TypeFormFieldDescriptor MultipleChoice(string id) => new TypeFormFieldDescriptor(id, "multiple_choice");
    public static TypeFormFieldDescriptor Dropdown(string id) => new TypeFormFieldDescriptor(id, "dropdown");
    public static TypeFormFieldDescriptor OpinionScale(string id) => new TypeFormFieldDescriptor(id, "opinion_scale");
    public static TypeFormFieldDescriptor Rating(string id) => new TypeFormFieldDescriptor(id, "rating");
    public static TypeFormFieldDescriptor Website(string id) => new TypeFormFieldDescriptor(id, "url");
    public static TypeFormFieldDescriptor Number(string id) => new TypeFormFieldDescriptor(id, "number");
    public static TypeFormFieldDescriptor Legal(string id) => new TypeFormFieldDescriptor(id, "legal");
    public static TypeFormFieldDescriptor Email(string id) => new TypeFormFieldDescriptor(id, "email");
  }
}