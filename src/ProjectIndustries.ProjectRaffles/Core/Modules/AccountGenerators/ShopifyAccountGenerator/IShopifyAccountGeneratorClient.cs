using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.AccountGenerators.ShopifyAccountGenerator
{
  public interface IShopifyAccountGeneratorClient
  {
    void GenerateHttpClient();
    Task<string> PostAccountInfoAsync(string baseurl, string firstname, string lastname, string email,
      CancellationToken ct);

    Task<bool> PostCaptchaAsync(string baseurl, string body, CancellationToken ct);
  }
}