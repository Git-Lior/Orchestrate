namespace Orchestrate.Data.Interfaces
{
    public interface IStringHasher
    {
        string Hash(string input);
        (bool success, bool needsUpgrade) CheckHash(string hash, string input);
    }
}
