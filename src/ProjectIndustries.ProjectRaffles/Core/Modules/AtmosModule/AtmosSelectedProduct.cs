namespace ProjectIndustries.ProjectRaffles.Core.Modules.AtmosModule
{
    public class AtmosSelectedProduct
    {
        public AtmosSelectedProduct(string releaseId, string size, string storeId)
        {
            ReleaseId = releaseId;
            Size = size;
            StoreId = storeId;
        }
        
        public string ReleaseId { get; }
        public string Size { get; }
        public string StoreId { get; }
    }
}