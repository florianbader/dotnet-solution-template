using Pulumi;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ContainerRegistry.Inputs;

namespace Infrastructure.Resources
{
    public class ContainerRegistryResource : AbstractResource
    {
        public ContainerRegistryResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "acr", nameHasHyphens: false, nameHasEnvironment: false)
        {
        }

        public void Build()
        {
            var config = new Config("containerregistry");

            _ = new Registry(Name, new RegistryArgs
            {
                RegistryName = Name,
                ResourceGroupName = ResourceGroupName,
                AdminUserEnabled = true,
                Sku = new SkuArgs
                {
                    Name = config.Require("sku"),
                },
            });
        }
    }
}
