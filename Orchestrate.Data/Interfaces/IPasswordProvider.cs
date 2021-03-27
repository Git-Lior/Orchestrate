namespace Orchestrate.Data.Interfaces
{
    public interface IPasswordProvider
    {
        string HashPassword(string password);
        (bool success, bool needsUpgrade) CheckHash(string passwordHash, string password);
        string GenerateTemporaryPassword(int size);
    }
}
