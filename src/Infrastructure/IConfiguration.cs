using System.Collections.Generic;
using Pulumi;

namespace Infrastructure.Resources
{
    public interface IConfiguration
    {
        IEnumerable<(string Key, Output<string>? Value)> Configuration { get; }
    }
}
