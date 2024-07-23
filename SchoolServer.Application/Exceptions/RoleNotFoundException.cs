namespace SchoolServer.Application.Exceptions;

public class RoleNotFoundException : DataAccessException
{
    public RoleNotFoundException() : base("This role not exist") { }

    private RoleNotFoundException(string message) : base(message) { }

    private RoleNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}
