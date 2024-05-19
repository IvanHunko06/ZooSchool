using SchoolServer.Core;

namespace SchoolServer.Application.Exceptions;

public class DataAccessException :SchoolServerException
{
    public DataAccessException() { }

    protected DataAccessException(string message) : base(message) { }

    protected DataAccessException(string message, Exception inner)
        : base(message, inner) { }
}
