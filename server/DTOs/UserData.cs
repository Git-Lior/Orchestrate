namespace Orchestrate.API.DTOs
{
    public class UserData
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsPasswordTemporary { get; set; }
    }

    public class UserDataWithToken : UserData
    {
        public string Token { get; set; }
    }

    public class UserDataWithTemporaryPassword : UserData
    {
        public string TemporaryPassword { get; set; }
    }
}
