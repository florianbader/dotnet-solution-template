using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.Authorization;

namespace Infrastructure
{
    public class Infrastructure : Stack
    {
        public Infrastructure()
        {
            var currentConfig = Output.Create(GetClientConfig.InvokeAsync());
            var currentUserObjectId = currentConfig.Apply(c => c.ClientId);
            var tenantId = currentConfig.Apply(c => c.TenantId);

            var resourceGroup = new ResourceGroupResource();

            var keyVault = new KeyVaultResource(resourceGroup, tenantId);

            var appServicePlan = new AppServicePlanResource(resourceGroup);
            appServicePlan.Build();

            var appService = new AppServiceResource(resourceGroup);

            // key vault needs to be build before any resource that accesses its secrets
            keyVault.Build();
            appService.AddConfiguration(keyVault, new[] { "KeyVaultName" });

            var staticWebsite = new StorageAccountResource(resourceGroup, "sw");
            staticWebsite.Build();
            staticWebsite.BuildStaticWebsite();

            var storageAccount = new StorageAccountResource(resourceGroup);
            storageAccount.Build();

            var applicationInsights = new ApplicationInsightsResource(resourceGroup);
            applicationInsights.Build();
            appService.AddConfiguration(applicationInsights, new[] { "APPINSIGHTS_INSTRUMENTATIONKEY" });

            var sqlDatabase = new SqlServerResource(resourceGroup, tenantId, currentUserObjectId);
            sqlDatabase.Build();
            keyVault.AddSecrets(sqlDatabase);
            appService.AddConfiguration(sqlDatabase, new[] { "DatabaseConnectionString" });

            appService.Build(appServicePlan);

            keyVault.AddAccessPolicy("appservice", appService.PrincipalId);
        }
    }
}
