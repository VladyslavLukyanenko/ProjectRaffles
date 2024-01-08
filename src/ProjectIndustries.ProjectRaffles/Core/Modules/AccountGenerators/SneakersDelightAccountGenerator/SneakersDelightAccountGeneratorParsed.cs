namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.SneakersDelightAccountGenerator
{
    public class SneakersDelightAccountGeneratorParsed
    {
        public SneakersDelightAccountGeneratorParsed(string formkey, string referer)
        {
            FormKey = formkey;
            Referer = referer;
        }
        public string FormKey { get; }
        public string Referer { get; }
    }
}