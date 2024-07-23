using SchoolServer.Core;
namespace SchoolServer.Application.Exceptions;

public class ServerApplicationException :SchoolServerException
{
    public ServerApplicationException() { }

    protected ServerApplicationException(string message) : base(message) { }

    protected ServerApplicationException(string message, Exception inner)
        : base(message, inner) { }
}
