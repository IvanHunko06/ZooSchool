namespace SchoolServer.Application.Exceptions;

public class FileExistException :ServerApplicationException
{
    public FileExistException() : base("File already exist") { }

    private FileExistException(string message) : base(message) { }

    private FileExistException(string message, Exception inner)
        : base(message, inner) { }
}
