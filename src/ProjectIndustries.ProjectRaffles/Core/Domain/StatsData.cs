using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class StatsData<T>
  {
    public StatsData()
    {
    }

    public StatsData(T data)
    {
      Data = data;
    }
    
    [JsonProperty("data")]
    public T Data { get; set; }
  }
}