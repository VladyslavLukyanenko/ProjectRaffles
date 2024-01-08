namespace ProjectIndustries.ProjectRaffles.Core.Domain.Modules
{
    public enum TaskStatus
    {
        [EnumDisplayValue("Entry Selected")]
        EntrySelected,
        [EnumDisplayValue("Submitting Entry")]
        SubmittingEntry,
        [EnumDisplayValue("Payment Declined")]
        PaymentDeclined,
        [EnumDisplayValue("Waiting for Results")]
        WaitingForResults
    }
}