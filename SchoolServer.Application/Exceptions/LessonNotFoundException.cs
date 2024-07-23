namespace SchoolServer.Application.Exceptions;

public class LessonNotFoundException : DataAccessException
{
    public LessonNotFoundException() : base("Lesson id not found in database") { }

    private LessonNotFoundException(string message) : base(message) { }

    private LessonNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}
