using System.Collections.Generic;
using System.Linq;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

namespace Infrastructure
{
    public class StorageAccountResource : AbstractResource, ISecrets
    {
        private StorageAccount _storageAccount = null!;

        public StorageAccountResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "stg", nameHasHyphens: false)
        {
        }

        public Output<string>? ConnectionString
            => Output.Tuple(ResourceGroupName, _storageAccount.Name)
                .Apply(names => Output.Create(ListStorageAccountKeys.InvokeAsync(new ListStorageAccountKeysArgs { AccountName = names.Item2, ResourceGroupName = names.Item1 }))
                    .Apply(s => $"DefaultEndpointsProtocol=https;AccountName={Name};AccountKey={s.Keys.First()};EndpointSuffix=core.windows.net"));

        public IEnumerable<(string Key, Output<string>? Value)> Secrets
            => new (string Key, Output<string>? Value)[] { (Key: "StorageConnectionString", Value: ConnectionString) };

        public void Build() => _storageAccount = new StorageAccount(Name, new StorageAccountArgs
        {
            AccountName = Name,
            ResourceGroupName = ResourceGroupName,
            Kind = Kind.StorageV2,
            AccessTier = AccessTier.Hot,
            Sku = new SkuArgs
            {
                Name = "Standard_LRS",
            },
        });
    }
}
