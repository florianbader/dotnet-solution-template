using System;

namespace Application;

public class GenericApplicationException : Exception
{
    public GenericApplicationException()
    {
    }

    public GenericApplicationException(string? message)
        : base(message)
    {
    }

    public GenericApplicationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
