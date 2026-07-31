using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.SettingsManagement.Entities;

public class SystemSetting : Entity<Guid>
{
    public string Key { get; private set; } = default!;
    public string Value { get; private set; } = default!;

    private SystemSetting() { }

    public SystemSetting(Guid id, string key, string value) : base(id)
    {
        Key = key;
        Value = value;
    }

    public void UpdateValue(string value)
    {
        Value = value;
    }
}
