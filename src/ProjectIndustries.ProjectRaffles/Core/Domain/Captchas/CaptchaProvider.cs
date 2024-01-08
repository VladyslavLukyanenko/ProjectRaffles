using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Captchas
{
  public class CaptchaProvider : Entity
  {
    private List<CaptchaKey> _keys = new List<CaptchaKey>();

    private CaptchaProvider()
    {
    }

    public CaptchaProvider(string providerName, CaptchaKey[] keys)
    {
      ProviderName = providerName;
      Keys = keys;
    }

    public CaptchaProvider(string providerName)
      : this(providerName, Array.Empty<CaptchaKey>())
    {
    }

    public event EventHandler KeysCountChanged;
    public event EventHandler KeysBalanceChanged;
    public string ProviderName { get; private set; }
    public IEnumerable<CaptchaKey> Keys
    {
      get => _keys.AsEnumerable();
      private set
      {
        _keys = value.ToList();
        foreach (var key in _keys)
        {
          key.BalanceChanged += DispatchUsableKeysChangedOnNoPositiveBalance;
        }
      }
    }

    public bool IsEmpty => !_keys.Any(_ => _.Balance > 0);
    public int MostIdleKeyUsageTimes => _keys.Where(_ => _.Balance > 0)
      .OrderBy(_ => _.UsedTimes)
      .Select(_ => _.UsedTimes)
      .FirstOrDefault();

    public bool AddKey(CaptchaKey key)
    {
      if (_keys.Any(k => k.Value == key.Value))
      {
        return false;
      }

      key.BalanceChanged += DispatchUsableKeysChangedOnNoPositiveBalance;
      _keys.Add(key);
      OnKeysCountChanged();
      return true;
    }

    public bool RemoveKey(CaptchaKey key)
    {
      var removed = _keys.Remove(key);
      if (removed)
      {
        OnKeysCountChanged();
        key.BalanceChanged -= DispatchUsableKeysChangedOnNoPositiveBalance;
      }
      
      return removed;
    }

    public CaptchaKey GetMostIdleKey()
    {
      if (_keys.Count == 0)
      {
        return null;
      }

      return _keys.Where(_ => _.Balance > 0).OrderBy(_ => _.UsedTimes).FirstOrDefault();
    }

    private void OnKeysCountChanged()
    {
      KeysCountChanged?.Invoke(this, EventArgs.Empty);
    }
    private void OnUsableKeysChanged()
    {
      KeysBalanceChanged?.Invoke(this, EventArgs.Empty);
    }

    private void DispatchUsableKeysChangedOnNoPositiveBalance(object sender, EventArgs e)
    {
      // var key = (CaptchaKey) sender;
      OnUsableKeysChanged();
    }
  }
}