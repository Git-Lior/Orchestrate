namespace Orchestrate.API.Services.Interfaces
{
    public interface IAdminService
    {
        string Authenticate(string password);
        void Verify(string token);
    }
}
