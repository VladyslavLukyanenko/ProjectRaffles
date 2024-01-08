namespace ProjectIndustries.ProjectRaffles.Core.Modules.EvgaModule
{
    public class EvgaParsedFields
    {
        public EvgaParsedFields(string viewState, string viewStateGenerator, string eventValidation)
        {
            ViewState = viewState;
            ViewStateGenerator = viewStateGenerator;
            EventValidation = eventValidation;
        }

        public string ViewState { get; }
        public string ViewStateGenerator { get; }
        public string EventValidation { get; }
    }
}