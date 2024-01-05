using System;
using System.Globalization;
using System.Linq;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.Authorization;

namespace Infrastructure;

public class Infrastructure : Stack
{
    private static readonly string[] Configurations = new[] { "DatabaseConnectionString" };

    public Infrastructure()
    {
        var currentConfig = Output.Create(GetClientConfig.InvokeAsync());
        var currentUserObjectId = currentConfig.Apply(c => c.ObjectId);
        var tenantId = currentConfig.Apply(c => c.TenantId);

        var resourceGroup = new ResourceGroupResource();

        var keyVault = new KeyVaultResource(resourceGroup, tenantId);

        var appServicePlan = new AppServicePlanResource(resourceGroup);
        appServicePlan.Build();

        var appService = new AppServiceResource(resourceGroup);

        // key vault needs to be build before any resource that accesses its secrets
        keyVault.Build();
        appService.AddConfiguration(keyVault, "KeyVaultName");

        var staticWebsite = new StorageAccountResource(resourceGroup, "sw");
        staticWebsite.Build();
        staticWebsite.BuildStaticWebsite();

        var storageAccount = new StorageAccountResource(resourceGroup);
        storageAccount.Build();

        var applicationInsights = new ApplicationInsightsResource(resourceGroup);
        applicationInsights.Build();
        appService.AddConfiguration(applicationInsights, "APPINSIGHTS_INSTRUMENTATIONKEY");

        var sqlDatabase = new SqlServerResource(resourceGroup, tenantId, currentUserObjectId);
        sqlDatabase.Build();
        keyVault.AddSecrets(sqlDatabase);
        appService.AddConfiguration(sqlDatabase, Configurations);

        appService.Build(appServicePlan);

        keyVault.AddAccessPolicy("appservice", appService.PrincipalId);

        var activeDirectory = new ActiveDirectoryResource(currentUserObjectId);
        var serviceGroup = activeDirectory.CreateGroup("servicegroup", appService.PrincipalId);

        ServicesGroupSid = GetDatabaseSid(serviceGroup.ObjectId);
    }

    [Output]
    public Output<string> ServicesGroupSid { get; set; }

    private static Output<string> GetDatabaseSid(Output<string> objectId)
        => objectId.Apply(o =>
        {
            var guid = Guid.Parse(o);
            return "0x" + string.Join(string.Empty, guid.ToByteArray().Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));
        });
}
