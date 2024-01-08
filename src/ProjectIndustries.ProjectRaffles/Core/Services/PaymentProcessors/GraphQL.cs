namespace ProjectIndustries.ProjectRaffles.Core.Services.PaymentProcessors
{
  public class GraphQL
  {
    public ClientSdkMetadata clientsdkmetadata { get; set; }
    public string operationName { get; set; }
    public string query { get; set; }
    public Variables variables { get; set; }
  }
}