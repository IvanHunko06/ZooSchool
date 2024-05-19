namespace SchoolServer.Core.Models
{
    public class User
    {
        private User(Guid id, string userName, string passwordHash, bool isBanned)
        {
            Id = id;
            UserName = userName;
            PasswordHash = passwordHash; 
            IsBanned = isBanned;
        }

        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string PasswordHash { get; private set;}
        public bool IsBanned { get; private set;}

        public static User Create(Guid id, string userName, string passwordHash, bool isBanned = false)
        {
            return new User(id, userName, passwordHash, isBanned);
        }

    }
}
