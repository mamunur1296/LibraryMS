using LibraryMS.Domain.SettingsManagement;
using LibraryMS.Domain.SettingsManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore.Repositories;

public class SystemSettingRepository : ISystemSettingRepository
{
    private readonly LibraryDbContext _context;

    public SystemSettingRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SystemSetting setting, CancellationToken cancellationToken = default)
    {
        await _context.SystemSettings.AddAsync(setting, cancellationToken);
    }

    public async Task<List<SystemSetting>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SystemSettings.ToListAsync(cancellationToken);
    }

    public async Task<SystemSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    public Task UpdateAsync(SystemSetting setting, CancellationToken cancellationToken = default)
    {
        _context.SystemSettings.Update(setting);
        return Task.CompletedTask;
    }
}
