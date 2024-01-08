namespace ProjectIndustries.ProjectRaffles.Core.Modules.UpThereModule
{
  public class UpThereProduct
  {
    public UpThereProduct(string name, string finalPrice, string country)
    {
      Name = name;
      FinalPrice = finalPrice;
      Country = country;
    }

    public string Name { get; }
    public string FinalPrice { get; }
    public string Country { get; }
  }
}