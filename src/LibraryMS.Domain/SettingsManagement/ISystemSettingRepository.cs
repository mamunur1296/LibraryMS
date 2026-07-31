using LibraryMS.Domain.SettingsManagement.Entities;

namespace LibraryMS.Domain.SettingsManagement;

public interface ISystemSettingRepository
{
    Task<SystemSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<List<SystemSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SystemSetting setting, CancellationToken cancellationToken = default);
    Task UpdateAsync(SystemSetting setting, CancellationToken cancellationToken = default);
}
