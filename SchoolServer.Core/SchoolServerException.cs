namespace SchoolServer.Core;

public class SchoolServerException : Exception
{
    public SchoolServerException() { }

    protected SchoolServerException(string message) :base(message) { }

    protected SchoolServerException(string message, Exception inner)
        : base(message, inner) { }

}
