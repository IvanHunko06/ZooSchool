namespace SchoolServer.Core.Models
{
    public class User
    {
        private User(string userName, string passwordHash)
        {
            Username = userName;
            PasswordHash = passwordHash; 
        }

        public string Username { get; private set; }
        public string PasswordHash { get; private set;}

        public static User Create(string userName, string passwordHash)
        {
            return new User(userName, passwordHash);
        }

    }
}
