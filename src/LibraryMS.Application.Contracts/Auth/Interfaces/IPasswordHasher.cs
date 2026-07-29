namespace LibraryMS.Application.Contracts.Auth.Interfaces;

// Interface for password hashing — implemented in Infrastructure.
public interface IPasswordHasher
{
    (string Hash, string Salt) Hash(string password);
    bool Verify(string password, string hash, string salt);
}
