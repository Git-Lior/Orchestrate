namespace Orchestrate.API.DTOs
{
    public class UserData
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class FullUserData
    {
        public bool IsPasswordTemporary { get; set; }
    }

    public class LoggedInUserData : FullUserData
    {
        public string Token { get; set; }
    }

    public class CreatedUserData : FullUserData
    {
        public string TemporaryPassword { get; set; }
    }
}
