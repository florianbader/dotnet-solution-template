using Infrastructure.Resources;
using Pulumi;

namespace Infrastructure
{
    public class Infrastructure : Stack
    {
        public Infrastructure()
        {
            var resourceGroup = new ResourceGroupResource();

            var appServicePlan = new AppServicePlanResource(resourceGroup);
            appServicePlan.Build();

            var appService = new AppServiceResource(resourceGroup);

            var keyVault = new KeyVaultResource(resourceGroup);
            keyVault.Build();

            var storageAccount = new StorageAccountResource(resourceGroup);
            storageAccount.Build();
            keyVault.AddSecrets(storageAccount);

            var applicationInsights = new ApplicationInsightsResource(resourceGroup);
            applicationInsights.Build();
            appService.AddConfiguration(applicationInsights);

            var sqlDatabase = new SqlServerResource(resourceGroup);
            sqlDatabase.Build();
            keyVault.AddSecrets(sqlDatabase);

            appService.AddConfiguration(keyVault);
            appService.Build(appServicePlan);

            keyVault.AddAccessPolicy(appService.TenantId, appService.PrincipalId);
        }
    }
}
