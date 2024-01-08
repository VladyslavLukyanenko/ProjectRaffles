﻿using System;

 namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface ILicenseKeyProvider
    {
        string CurrentLicenseKey { get; }
        IObservable<string> LicenseKey { get; }
        void Invalidate();
        void UseLicenseKey(string licenseKey);
    }
}