using System;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class Account : Entity
  {
    [BsonCtor, JsonConstructor]
    public Account(Guid id, string email, string password, string accessToken = null)
      : base(id)
    {
      Email = email;
      Password = password;
      AccessToken = accessToken;
    }

    public Account(string email, string password, string accessToken = null)
      : this(Guid.NewGuid(), email, password, accessToken)
    {
    }

    public string Email { get; private set; }
    public string Password { get; private set; }
    public string AccessToken { get; set; }
  }
}