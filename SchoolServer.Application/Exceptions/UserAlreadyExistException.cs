namespace SchoolServer.Application.Exceptions;
public class UserAlreadyExistException : DataAccessException
{
    public UserAlreadyExistException() : base("Such user already exists in the database") { }

    private UserAlreadyExistException(string message) : base(message) { }

    private UserAlreadyExistException(string message, Exception inner)
        : base(message, inner) { }
}
