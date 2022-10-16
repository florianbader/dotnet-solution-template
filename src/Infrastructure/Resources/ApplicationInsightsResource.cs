using System.Collections.Generic;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.Insights;

namespace Infrastructure;

public class ApplicationInsightsResource : AbstractResource, IConfiguration
{
    private Component? _applicationInsights;

    public ApplicationInsightsResource(ResourceGroupResource resourceGroup)
        : base(resourceGroup, "ai")
    {
    }

    public IEnumerable<(string Key, Output<string>? Value)> Configuration
        => new[] { ("APPINSIGHTS_INSTRUMENTATIONKEY", InstrumentationKey) };

    public Output<string>? InstrumentationKey => _applicationInsights?.InstrumentationKey;

    public void Build() => _applicationInsights = new Component(Name, new ComponentArgs
    {
        ResourceName = Name,
        ResourceGroupName = ResourceGroupName,
        ApplicationType = ApplicationType.Web,
        Kind = "web",
    });
}
