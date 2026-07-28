namespace LibraryMS.Domain.IdentityManagement;

public sealed class UserManager
{
    public void RecordLogin(User user)
    {
        user.RecordLogin();
    }
}
