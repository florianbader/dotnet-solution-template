using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.AzureNative.Devices;
using Pulumi.AzureNative.Devices.Inputs;

namespace Infrastructure.Resources;

public class IoTHubResource : AbstractResource, ISecrets
{
    private IotHubResource? _iotHub;

    public IoTHubResource(ResourceGroupResource resourceGroupResource)
        : base(resourceGroupResource, "ioth")
    {
    }

    public Output<string>? ConnectionString
        => _iotHub?.Properties.Apply(p =>
        {
            var key = p.AuthorizationPolicies.First(p => p.KeyName == "iothubowner").PrimaryKey;
            return $"HostName={Name}.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=${key}";
        });

    public IEnumerable<(string Key, Output<string>? Value)> Secrets
        => new (string Key, Output<string>? Value)[]
        {
            (Key: "IoTHubConnectionString", Value: ConnectionString),
            (Key: "EventHubConnectionString", Value: _iotHub!.Properties.Apply(p => p.EventHubEndpoints!.First().Value.Endpoint)),
        };

    public void Build()
    {
        var config = new Config("iothub");

        _iotHub = new IotHubResource(Name, new IotHubResourceArgs
        {
            ResourceName = Name,
            ResourceGroupName = ResourceGroupName,
            Sku = new IotHubSkuInfoArgs
            {
                Name = config.Require("name"),
                Capacity = 1,
            },
            Properties = new IotHubPropertiesArgs
            {
                EventHubEndpoints = new InputMap<EventHubPropertiesArgs>()
                {
                    {
                        "events",
                        new EventHubPropertiesArgs
                        {
                            PartitionCount = 32,
                            RetentionTimeInDays = 5,
                        }
                    },
                },
            },
        });
    }
}
