using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
    public interface ISettingsService
    {
        Task<T> ReadSettingsOrDefaultAsync<T>(string name, Func<T> defaultFactory = null,
            CancellationToken ct = default);

        Task WriteSettingsAsync<T>(string name, T settings, CancellationToken ct = default);
    }
}