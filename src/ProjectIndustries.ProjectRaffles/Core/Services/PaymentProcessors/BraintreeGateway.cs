namespace ProjectIndustries.ProjectRaffles.Core.Services.PaymentProcessors
{
    public class BraintreeGateway
    {
        public additionalinfo additional_info { get; set; }
        public string amount { get; set; }
        public string authorizationFingerprint { get; set; }
        public bool challenge_requested { get; set; }
        public bool exemption_requested { get; set; }
    }
}