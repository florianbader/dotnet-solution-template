using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;

namespace Infrastructure
{
    public class KeyVaultResource : AbstractResource, IConfiguration
    {
        private readonly InputList<AccessPolicyEntryArgs> _accessPolicies = new();
        private readonly Dictionary<string, Secret> _secrets = new();
        private Vault? _keyVault;

        public KeyVaultResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "kv")
        {
        }

        public IEnumerable<(string Key, Output<string>? Value)> Configuration
            => _secrets.Keys
                .Select<string, (string Key, Output<string>? Value)>(s
                    => (Key: s.Replace("--", "__", StringComparison.Ordinal), Value: _secrets[s].Properties.Apply(p => $"@Microsoft.KeyVault(SecretUri={p.SecretUriWithVersion})")))
                .Concat(new[]
                {
                    (Key: "KeyVaultName", Value: _keyVault?.Name),
                });

        public Output<string>? this[string key] => _secrets[key].Properties.Apply(p => $"@Microsoft.KeyVault(SecretUri={p.SecretUriWithVersion})");

        public void AddAccessPolicy(Output<string>? tenantId, Output<string>? principalId)
        {
            if (tenantId == null)
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (principalId == null)
            {
                throw new ArgumentNullException(nameof(principalId));
            }

            if (_keyVault == null)
            {
                throw new InvalidOperationException("Key vault needs to be build before adding secrets.");
            }

            _accessPolicies.Add(
                new AccessPolicyEntryArgs
                {
                    TenantId = tenantId,
                    ObjectId = principalId,
                    Permissions = new PermissionsArgs
                    {
                        Secrets =
                        {
                            "get", "list", "set", "delete",
                        },
                    },
                });
        }

        public void AddSecrets(ISecrets secrets)
        {
            foreach (var secret in secrets.Secrets)
            {
                if (secret.Value != null)
                {
                    SetSecret(secret.Key, secret.Value);
                }
            }
        }

        public void Build()
        {
            var projectConfig = new Config("project");
            var config = new Config("keyvault");

            _keyVault = new Vault(Name, new VaultArgs
            {
                VaultName = Name,
                ResourceGroupName = ResourceGroupName,
                Properties = new VaultPropertiesArgs
                {
                    Sku = new SkuArgs
                    {
                        Family = SkuFamily.A,
                        Name = config.Require("sku") == "standard" ? SkuName.Standard : SkuName.Premium,
                    },
                    TenantId = projectConfig.Require("tenantId"),
                    AccessPolicies = _accessPolicies,
                },
            });
        }

        public Secret SetSecret(string secretName, string secretValue)
        {
            if (_keyVault == null)
            {
                throw new InvalidOperationException("Key vault needs to be build before adding secrets.");
            }

            var secret = new Secret(secretName, new SecretArgs
            {
                SecretName = secretName,
                VaultName = _keyVault.Name,
                Properties = new SecretPropertiesArgs
                {
                    Value = secretValue,
                },
            });

            _secrets.Add(secretName, secret);

            return secret;
        }

        public Secret SetSecret(string secretName, Output<string> secretValue)
        {
            if (_keyVault == null)
            {
                throw new InvalidOperationException("Key vault needs to be build before adding secrets.");
            }

            var secret = new Secret(secretName, new SecretArgs
            {
                SecretName = secretName,
                VaultName = _keyVault.Name,
                ResourceGroupName = ResourceGroupName,
                Properties = new SecretPropertiesArgs
                {
                    Value = secretValue,
                },
            });

            _secrets.Add(secretName, secret);

            return secret;
        }
    }
}
