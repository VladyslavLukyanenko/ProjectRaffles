using DynamicData.Binding;

using LiteDB;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class AccountGroup : Entity
  {
    [BsonCtor, JsonConstructor]
    public AccountGroup(Guid id, string name)
      : base(id)
    {
      Name = name.Trim();
    }

    // [BsonCtor]
    public AccountGroup(string name, IEnumerable<Account> accounts = null)
      : this(Guid.Empty, name)
    {
      Accounts = new ObservableCollectionExtended<Account>(accounts ?? Array.Empty<Account>());
    }

    public string Name { get; private set; }

    public ObservableCollectionExtended<Account> Accounts { get; private set; } =
      new ObservableCollectionExtended<Account>();

    public void Remove(Account acc)
    {
      Accounts.Remove(acc);
    }
  }
}