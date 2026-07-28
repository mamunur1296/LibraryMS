using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared.Enums;

namespace LibraryMS.Domain.IdentityManagement;

public sealed class UserManager
{
    private readonly IGuidGenerator _guidGenerator;

    public UserManager(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    public void RecordLogin(User user)
    {
        user.RecordLogin();
    }

    public User Create(string username, string email, string passwordHash, string salt, UserRole role)
    {
        return new User(_guidGenerator.Create(), username, email, passwordHash, salt, role);
    }
}
