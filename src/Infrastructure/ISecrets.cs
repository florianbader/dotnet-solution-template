using System.Collections.Generic;
using Pulumi;

namespace Infrastructure;

public interface ISecrets
{
    IEnumerable<(string Key, Output<string>? Value)> Secrets { get; }
}
