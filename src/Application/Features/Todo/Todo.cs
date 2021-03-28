using System;

namespace Application.Features.Todo
{
    public record Todo
    {
        public Todo(string name) => Name = name;

        public string Id { get; init; } = Guid.NewGuid().ToString();

        public string Name { get; init; }

        public bool IsDone { get; init; }
    }
}
