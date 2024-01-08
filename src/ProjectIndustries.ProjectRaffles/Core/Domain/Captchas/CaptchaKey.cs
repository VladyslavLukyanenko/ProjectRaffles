using System;
using System.Threading;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Captchas
{
  public class CaptchaKey
  {
    private int _usedTimes;
    private decimal _balance;

    // for litedb
    private CaptchaKey()
    {
    }

    public CaptchaKey(int usedTimes, string value)
    {
      _usedTimes = usedTimes;
      Value = value.Trim();
    }

    public CaptchaKey(string value)
      : this(0, value)
    {
    }

    public int UsedTimes
    {
      get => _usedTimes;
      private set => _usedTimes = value;
    }

    public string Value { get; private set; }

    public decimal Balance
    {
      get => _balance;
      set
      {
        if (_balance == value)
        {
          return;
        }

        _balance = value;
        OnBalanceChange();
      }
    }

    public event EventHandler BalanceChanged;

    public void Used()
    {
      Interlocked.Increment(ref _usedTimes);
    }

    public void FailedToUse()
    {
      Interlocked.Decrement(ref _usedTimes);
    }

    private void OnBalanceChange()
    {
      BalanceChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}