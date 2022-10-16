using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace Infrastructure;

public class AppServicePlanResource : AbstractResource
{
    private AppServicePlan? _appServicePlan;

    public AppServicePlanResource(ResourceGroupResource resourceGroup)
        : base(resourceGroup, "asp")
    {
    }

    public Output<string>? Id => _appServicePlan?.Id;

    public void Build()
    {
        var config = new Config("appservice");

        _appServicePlan = new AppServicePlan(Name, new AppServicePlanArgs
        {
            Name = Name,
            ResourceGroupName = ResourceGroupName,
            Kind = "Linux",
            Reserved = true,
            Sku = new SkuDescriptionArgs
            {
                Tier = config.Require("tier"),
                Size = config.Require("size"),
                Name = config.Require("size"),
            },
        });
    }
}
