namespace SchoolServer.Application.Exceptions;

public class TestNotFoundException : DataAccessException
{
    public TestNotFoundException() : base("Test not found in database") { }

    private TestNotFoundException(string message) : base(message) { }

    private TestNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}
