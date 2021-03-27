namespace Orchestrate.API.Services.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateUserToken(int userId);
        string GenerateAdminToken();
    }
}
