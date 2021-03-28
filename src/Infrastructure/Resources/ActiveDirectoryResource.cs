using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Resources;
using Pulumi;

namespace Infrastructure
{
    public class ActiveDirectoryResource : ISecrets, IConfiguration
    {
        public ActiveDirectoryResource()
        {
            var config = new Config("azureactivedirectory");

            ClientId = config.Require("clientid");
            ClientSecret = config.GetSecret("clientsecret");
            Domain = config.Require("domain");
            TenantId = config.Require("tenantId");
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
    }
}
