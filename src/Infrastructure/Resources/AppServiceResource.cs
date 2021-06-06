using System;
using System.Collections.Generic;
using System.Linq;
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

        public Output<string> PrincipalId => _appService?.Identity.Apply(i => i?.PrincipalId ?? "11111111-1111-1111-1111-111111111111") // workaround for preview
            ?? throw new InvalidOperationException("Could not find app service principal id");

        public Output<string> TenantId => _appService?.Identity.Apply(i => i?.TenantId ?? "11111111-1111-1111-1111-111111111111") // workaround for preview
            ?? throw new InvalidOperationException("Could not find app service tenant id");

        public void AddConfiguration(string key, Output<string>? value)
        {
            if (value == null)
            {
                return;
            }

            _appSettings.Add(new NameValuePairArgs { Name = key, Value = value });
        }

        public void AddConfiguration(IConfiguration configurationProvider, params string[] configurations)
        {
            var configurationKeys = new HashSet<string>(configurations);
            var configuration = configurationProvider.Configuration.Where(c => configurationKeys.Contains(c.Key));

            foreach (var (key, value) in configuration)
            {
                AddConfiguration(key, value);
            }
        }

        public void Build(AppServicePlanResource appServicePlanResource)
        {
            _ = appServicePlanResource.Id ?? throw new InvalidOperationException("App service plan was not build");

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
