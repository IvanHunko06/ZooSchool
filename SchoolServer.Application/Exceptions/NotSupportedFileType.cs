namespace SchoolServer.Application.Exceptions;

public class NotSupportedFileType : ServerApplicationException
{
    public NotSupportedFileType() : base("Incorrect file type") { }

    private NotSupportedFileType(string message) : base(message) { }

    private NotSupportedFileType(string message, Exception inner)
        : base(message, inner) { }
}
