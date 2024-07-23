namespace SchoolServer.Application.Exceptions;

public class UserNotFoundException : DataAccessException
{
    public UserNotFoundException() : base("User not found in database") { }

    private UserNotFoundException(string message) : base(message) { }

    private UserNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}
