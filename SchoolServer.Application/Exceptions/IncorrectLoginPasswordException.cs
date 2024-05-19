namespace SchoolServer.Application.Exceptions;

public class IncorrectLoginPasswordException :ServerApplicationException
{
    public IncorrectLoginPasswordException() : base("Login password is incorrect") { }

    private IncorrectLoginPasswordException(string message) : base(message) { }

    private IncorrectLoginPasswordException(string message, Exception inner)
        : base(message, inner) { }
}
