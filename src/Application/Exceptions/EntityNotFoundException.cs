using System;
using System.Runtime.Serialization;

namespace Application;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string? message)
        : base(message)
    {
    }

    public EntityNotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
