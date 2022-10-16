using System.Threading.Tasks;
using Pulumi;

namespace Infrastructure;

public static class Program
{
    public static Task<int> Main() => Deployment.RunAsync<Infrastructure>();
}
