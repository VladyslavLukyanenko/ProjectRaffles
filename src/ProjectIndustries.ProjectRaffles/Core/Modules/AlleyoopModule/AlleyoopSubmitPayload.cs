namespace ProjectIndustries.ProjectRaffles.Core.Modules.AlleyoopModule
{
    public class AlleyoopSubmitPayload
    {
        public AlleyoopSubmitPayload(string raffleUrl, AddressFields addressFields, string email, string payment, string furiganaFirst,
            string furiganaLast, string deliveryTime, string itemcode, string building)
        {
            RaffleUrl = raffleUrl;
            AddressFields = addressFields;
            Email = email;
            Payment = payment;
            FuriganaFirst = furiganaFirst;
            FuriganaLast = furiganaLast;
            DeliveryTime = deliveryTime;
            ItemCode = itemcode;
            Building = building;
        }
        public string RaffleUrl { get; }
        public AddressFields AddressFields { get; }
        public string Email { get; }
        public string Payment { get; }
        public string FuriganaFirst { get; }
        public string FuriganaLast { get; }
        public string DeliveryTime { get;  }
        public string ItemCode { get; }
        public string Building { get; }
    }
}