using System;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using ManagedServiceIdentityArgs = Pulumi.AzureNative.Web.Inputs.ManagedServiceIdentityArgs;

namespace Infrastructure
{
    public class AppServiceResource : AbstractResource
    {
        private readonly InputList<NameValuePairArgs> _appSettings = new InputList<NameValuePairArgs>();
        private WebApp? _appService;

        public AppServiceResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "app")
        {
        }

        public Output<string>? PrincipalId => _appService?.Identity.Apply(i => i?.PrincipalId ?? string.Empty);

        public Output<string>? TenantId => _appService?.Identity.Apply(i => i?.TenantId ?? string.Empty);

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

        public void Build(AppServicePlanResource appServicePlanResource)
        {
            if (appServicePlanResource.Id == null)
            {
                throw new InvalidOperationException("App service plan was not build");
            }

            _appService = new WebApp(Name, new WebAppArgs
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                ServerFarmId = appServicePlanResource.Id,
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
