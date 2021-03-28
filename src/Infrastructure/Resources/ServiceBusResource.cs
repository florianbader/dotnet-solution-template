using System.Collections.Generic;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.ServiceBus;
using Pulumi.AzureNative.ServiceBus.Inputs;

namespace Infrastructure
{
    public class ServiceBusResource : AbstractResource, ISecrets
    {
        private Namespace _serviceBusNamespace = null!;

        public ServiceBusResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "sb", nameHasHyphens: false)
        {
        }

        public Output<string>? ConnectionString
            => Output.Tuple(ResourceGroupName, _serviceBusNamespace.Name)
                .Apply(names => Output.Create(ListNamespaceKeys.InvokeAsync(new ListNamespaceKeysArgs { NamespaceName = names.Item2, AuthorizationRuleName = "RootManageSharedAccessKey", ResourceGroupName = names.Item1 }))
                    .Apply(s => s.PrimaryConnectionString));

        public IEnumerable<(string Key, Output<string>? Value)> Secrets
            => new[] { (Key: "ServiceBusConnectionString", Value: ConnectionString) };

        public void Build()
        {
            var config = new Config("servicebus");

            _serviceBusNamespace = new Namespace(Name, new NamespaceArgs
            {
                NamespaceName = Name,
                ResourceGroupName = ResourceGroupName,
                Sku = new SBSkuArgs
                {
                    Name = config.Require("sku") == "basic" ? SkuName.Basic : SkuName.Standard,
                    Tier = config.Require("sku") == "basic" ? SkuTier.Basic : SkuTier.Standard,
                    Capacity = 0,
                },
            });
        }
    }
}
