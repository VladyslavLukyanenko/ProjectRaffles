using System;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Emails
{
  public class SmtpConfig : Entity
  {
    [BsonCtor]
    [JsonConstructor]
    public SmtpConfig(string host, int port/*, bool useSsl*/)
      : base(Guid.NewGuid())
    {
      Host = host;
      Port = port;
      // UseSsl = useSsl;
    }

    public string Host { get; private set; }
    public int Port { get; private set; }
    // public bool UseSsl { get; private set; }
  }
}