namespace SchoolServer.Application.Exceptions
{
    public class UserBannedException :ServerApplicationException
    {
        public UserBannedException() : base("User banned") { }

        private UserBannedException(string message) : base(message) { }

        private UserBannedException(string message, Exception inner)
            : base(message, inner) { }
    }
}
