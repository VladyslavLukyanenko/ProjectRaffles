using System;
using System.Reactive.Subjects;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public class LicenseKeyProvider : ILicenseKeyProvider
    {
        private readonly BehaviorSubject<string> _licenseKey = new BehaviorSubject<string>(null);

        public LicenseKeyProvider()
        {
            LicenseKey = _licenseKey;
        }

        public string CurrentLicenseKey => _licenseKey.Value;
        public IObservable<string> LicenseKey { get; }
        public void Invalidate()
        {
            _licenseKey.OnNext(null);
        }

        public void UseLicenseKey(string licenseKey)
        {
            _licenseKey.OnNext(licenseKey);
        }
    }
}