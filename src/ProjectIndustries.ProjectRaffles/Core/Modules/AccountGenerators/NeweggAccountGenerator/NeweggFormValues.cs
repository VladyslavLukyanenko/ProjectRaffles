namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.NeweggAccountGenerator
{
    public class NeweggFormValues
    {
        public NeweggFormValues(string firstNameForm, string lastNameForm, string emailForm, string passwordForm,
            string fuzzyKey)
        {
            FirstNameKey = firstNameForm;
            LastNameKey = lastNameForm;
            EmailKey = emailForm;
            PasswordKey = passwordForm;
            FuzzyKey = fuzzyKey;
        }
        public string FirstNameKey { get; set; }
        public string LastNameKey { get; set; }
        public string EmailKey { get; set; }
        public string PasswordKey { get; set; }
        public string FuzzyKey { get; set; }
    }
}