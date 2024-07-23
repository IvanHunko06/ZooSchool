namespace SchoolServer.Application.Exceptions;

public class IncorrectPasswordException :ServerApplicationException
{
    public IncorrectPasswordException() : base("Login password is incorrect") { }

    private IncorrectPasswordException(string message) : base(message) { }

    private IncorrectPasswordException(string message, Exception inner)
        : base(message, inner) { }
}
