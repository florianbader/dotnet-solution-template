using System;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace Infrastructure
{
    public class FunctionAppResource : AbstractResource
    {
        private readonly InputList<NameValuePairArgs> _appSettings = new InputList<NameValuePairArgs>();
        private WebApp? _functionApp;

        public FunctionAppResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "func")
        {
        }

        public Output<string?>? PrincipalId => _functionApp?.Identity.Apply(i => i?.PrincipalId);

        public Output<string?>? TenantId => _functionApp?.Identity.Apply(i => i?.TenantId);

        public void AddConfiguration(string key, Output<string>? value)
        {
            if (value == null)
            {
                return;
            }

            _appSettings.Add(new NameValuePairArgs { Name = key, Value = value });
        }

        public void AddConfiguration(IConfiguration configurationProvider)
        {
            foreach (var (key, value) in configurationProvider.Configuration)
            {
                AddConfiguration(key, value);
            }
        }

        public void Build(AppServicePlanResource appServicePlanResource, StorageAccountResource storageAccountResource)
        {
            _ = appServicePlanResource.Id ?? throw new InvalidOperationException("App service plan was not build.");
            _ = storageAccountResource.ConnectionString ?? throw new InvalidOperationException("Storage account was not build.");

            _appSettings.Add(new NameValuePairArgs { Name = "FUNCTIONS_EXTENSION_VERSION", Value = "~3" });
            _appSettings.Add(new NameValuePairArgs { Name = "FUNCTIONS_WORKER_RUNTIME", Value = "dotnet" });
            _appSettings.Add(new NameValuePairArgs { Name = "AzureWebJobsStorage", Value = storageAccountResource.ConnectionString });
            _appSettings.Add(new NameValuePairArgs { Name = "WEBSITES_ENABLE_APP_SERVICE_STORAGE", Value = "false" });

            _functionApp = new WebApp(Name, new WebAppArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                ServerFarmId = appServicePlanResource.Id,
                Kind = "functionapp",
                HttpsOnly = true,
                Identity = new ManagedServiceIdentityArgs
                {
                    Type = ManagedServiceIdentityType.SystemAssigned,
                },
                SiteConfig = new SiteConfigArgs
                {
                    LinuxFxVersion = "DOTNETCORE|5.0",
                    AlwaysOn = true,
                    Http20Enabled = true,
                    WebSocketsEnabled = true,
                    AppSettings = _appSettings,
                },
            });
        }
    }
}
