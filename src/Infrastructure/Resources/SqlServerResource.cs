using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Sql.Inputs;

namespace Infrastructure
{
    public class SqlServerResource : AbstractResource, ISecrets
    {
        private Output<string>? _password;

        public SqlServerResource(ResourceGroupResource resourceGroup)
            : base(resourceGroup, "sqls")
        {
        }

        public string? ConnectionString { get; private set; }

        public IEnumerable<(string Key, Output<string>? Value)> Secrets
            => new (string Key, Output<string>? Value)[]
            {
                (Key: "DatabaseConnectionString", Value: Output<string>.Create(Task.FromResult(ConnectionString ?? string.Empty))),
                (Key: "DatabasePassword", Value: _password),
            };

        public void Build()
        {
            var config = new Config("sqlserver");

            var databaseName = GetResourceName("sqld");
            var username = "ServerAdmin";
            _password = config.GetSecret("password");

            if (_password == null)
            {
                throw new InvalidOperationException("SQL Server password configuration is missing.");
            }

            ConnectionString = $"Server=tcp:${Name}.database.windows.net;Initial Catalog=${databaseName};User Id={username};Password={_password};Min Pool Size=0;Max Pool Size=30;Persist Security Info=true;";

            var sqlServer = new Server(Name, new ServerArgs
            {
                ServerName = Name,
                ResourceGroupName = ResourceGroupName,
                Version = "12.0",
                AdministratorLogin = username,
                AdministratorLoginPassword = _password,
            });

            _ = new Database(databaseName, new DatabaseArgs
            {
                DatabaseName = sqlServer.Name,
                ResourceGroupName = ResourceGroupName,
                ServerName = Name,
                Sku = new SkuArgs
                {
                    Name = config.Require("serviceObjectiveName"),
                    Tier = config.Require("edition"),
                },
            });

            _ = new FirewallRule("Allow all Azure services", new FirewallRuleArgs
            {
                ServerName = sqlServer.Name,
                ResourceGroupName = ResourceGroupName,
                Name = "Allow all Azure services",
                StartIpAddress = "0.0.0.0",
                EndIpAddress = "0.0.0.0",
            });
        }
    }
}
