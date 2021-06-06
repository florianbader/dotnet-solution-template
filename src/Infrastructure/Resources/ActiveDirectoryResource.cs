using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureAD;
using Config = Pulumi.Config;

namespace Infrastructure
{
    public class ActiveDirectoryResource : ISecrets, IConfiguration
    {
        private readonly Output<string> _currentUserObjectId;

        public ActiveDirectoryResource(Output<string> currentUserObjectId)
        {
            var config = new Config("azureactivedirectory");

            ClientId = config.Require("clientid");
            ClientSecret = config.GetSecret("clientsecret");
            Domain = config.Require("domain");
            TenantId = config.Require("tenantId");
            _currentUserObjectId = currentUserObjectId;
        }

        public string ClientId { get; }

        public Output<string>? ClientSecret { get; }

        public IEnumerable<(string Key, Output<string>? Value)> Configuration
            => new (string Key, Output<string>? Value)[]
            {
                (Key: "AzureActiveDirectoryBearer__ApplicationIdUri", Value: Output<string>.Create(Task.FromResult($"api://{ClientId}"))),
                (Key: "AzureActiveDirectoryBearer__ClientId", Value: Output<string>.Create(Task.FromResult(ClientId))),
                (Key: "AzureActiveDirectoryBearer__Domain", Value: Output<string>.Create(Task.FromResult(Domain))),
                (Key: "AzureActiveDirectoryBearer__Instance", Value: Output<string>.Create(Task.FromResult("https://login.microsoftonline.com/"))),
                (Key: "AzureActiveDirectoryBearer__TenantId", Value: Output<string>.Create(Task.FromResult(TenantId))),
            };

        public string Domain { get; }

        public IEnumerable<(string Key, Output<string>? Value)> Secrets
            => new (string Key, Output<string>? Value)[]
            {
                (Key: "AzureActiveDirectoryClientSecret", Value: ClientSecret),
            };

        public string TenantId { get; }

        public Group CreateGroup(string name, params Output<string>[] identities)
        {
            var projectConfig = new Config("project");

            var productName = projectConfig.Require("productName");
            var environment = projectConfig.Require("environment");

            return new Group(name, new GroupArgs
            {
                DisplayName = $"{productName}-{environment}-{name}",
                Members = identities,
                Owners = _currentUserObjectId,
            });
        }
    }
}
