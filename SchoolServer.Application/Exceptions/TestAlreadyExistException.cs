namespace SchoolServer.Application.Exceptions;

public class TestAlreadyExistException : DataAccessException
{
    public TestAlreadyExistException() : base("Such test already exists in the database") { }

    private TestAlreadyExistException(string message) : base(message) { }

    private TestAlreadyExistException(string message, Exception inner)
        : base(message, inner) { }
}
