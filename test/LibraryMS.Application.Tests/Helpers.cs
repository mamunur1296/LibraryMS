using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Enums;
using System.Reflection;

namespace LibraryMS.Application.Tests;

public static class Helpers
{
    public static User CreateUser(Guid id, string username, string email, string passwordHash, string salt, UserRole role)
    {
        var constructor = typeof(User).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(Guid), typeof(string), typeof(string), typeof(string), typeof(string), typeof(UserRole), typeof(Guid?) },
            null);

        if (constructor == null)
            throw new Exception("Could not find internal constructor for User.");

        return (User)constructor.Invoke(new object[] { id, username, email, passwordHash, salt, role, null });
    }
}
