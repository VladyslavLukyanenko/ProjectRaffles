using System;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Modules.SvdModule
{
  public class TestPayload
  {
    [JsonProperty(nameof(Prop1)), BsonField(nameof(Prop1))]
    public string Prop1 { get; set; }

    [JsonProperty(nameof(Prop2)), BsonField(nameof(Prop2))]
    public DateTime Prop2 { get; set; }

    [JsonProperty(nameof(Prop3)), BsonField(nameof(Prop3))]
    public int Prop3 { get; set; }

    public override string ToString()
    {
      return  $"{Prop1}_{Prop3}_" + Prop2;
    }
  }
}