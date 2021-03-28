using System;
using System.Collections.Generic;
using System.Linq;

namespace Application
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException()
        {
        }

        public InvalidInputException(string? message)
            : base(message)
        {
        }

        public InvalidInputException(IEnumerable<ValidationResult> errors)
            : base(string.Join(Environment.NewLine, errors)) => Errors = errors;

        public InvalidInputException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        public IEnumerable<ValidationResult> Errors { get; set; } = Enumerable.Empty<ValidationResult>();
    }
}
