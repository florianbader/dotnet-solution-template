using System;
using Pulumi;
using Pulumi.AzureNative.Resources;

namespace Infrastructure.Resources
{
    public class ResourceGroupResource
    {
        public ResourceGroupResource(bool nameHasEnvironment = true)
        {
            var azureNativeConfig = new Config("azure-native");
            var azureConfig = new Config("azure");
            var projectConfig = new Config("project");

            ProductName = projectConfig.Require("productName");
            Environment = azureConfig.Require("environment");
            Location = azureNativeConfig.Require("location");
            Name = GetResourceName("rg", nameHasEnvironment: nameHasEnvironment);

            ResourceGroup = new ResourceGroup(
                Name,
                new ResourceGroupArgs
                {
                    ResourceGroupName = Name,
                },
                new CustomResourceOptions
                {
                    Protect = true,
                });
        }

        public string Environment { get; }

        public string Location { get; }

        public string LocationAbbreviation
            => Location switch
            {
                "WestEurope" => "euw",
                _ => throw new ArgumentException("Invalid location"),
            };

        public string Name { get; }

        public string ProductName { get; }

        public ResourceGroup ResourceGroup { get; }

        public string GetResourceName(string name, bool nameHasHyphens = true, bool nameHasEnvironment = true)
        {
            if (nameHasEnvironment)
            {
                return nameHasHyphens
                    ? $"{ProductName}-{Environment}-{LocationAbbreviation}-{name}"
                    : $"{ProductName}{Environment}{LocationAbbreviation}{name}";
            }
            else
            {
                return nameHasHyphens
                    ? $"{ProductName}-{LocationAbbreviation}-{name}"
                    : $"{ProductName}{LocationAbbreviation}{name}";
            }
        }
    }
}
