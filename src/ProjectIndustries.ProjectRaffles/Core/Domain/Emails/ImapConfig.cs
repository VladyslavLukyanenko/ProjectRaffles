using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Emails
{
  public class ImapConfig : Entity
  {
    [BsonCtor]
    [JsonConstructor]
    public ImapConfig(string host, int port, bool useSsl)
    {
      Host = host;
      Port = port;
      UseSsl = useSsl;
    }

    public string Host { get; private set; }
    public int Port { get; private set; }
    public bool UseSsl { get; private set; }
  }
}