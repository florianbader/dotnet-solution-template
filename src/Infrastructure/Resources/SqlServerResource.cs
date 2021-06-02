using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Resources;
using Pulumi;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Sql.Inputs;

namespace Infrastructure
{
    public class SqlServerResource : AbstractResource, ISecrets, IConfiguration
    {
        private readonly Output<string> _currentUserObjectId;
        private Output<string>? _password;
        private string? _connectionString;

        public SqlServerResource(ResourceGroupResource resourceGroup, Output<string> currentUserObjectId)
            : base(resourceGroup, "sqls")
        {
            _currentUserObjectId = currentUserObjectId;
        }

        public IEnumerable<(string Key, Output<string>? Value)> Configuration
            => new (string Key, Output<string>? Value)[]
            {
                (Key: "DatabaseConnectionString", Value: Output<string>.Create(Task.FromResult(_connectionString ?? throw new InvalidOperationException("SQL Server was not build yet")))),
            };

        public IEnumerable<(string Key, Output<string>? Value)> Secrets
            => new (string Key, Output<string>? Value)[]
            {
                (Key: "DatabasePassword", Value: _password),
            };

        public void Build()
        {
            var projectConfig = new Config("project");
            var config = new Config("sqlserver");

            var databaseName = GetResourceName("sqld");
            var username = "ServerAdmin";
            _password = config.GetSecret("password") ?? throw new InvalidOperationException("SQL Server password configuration is missing.");

            _connectionString = $"Server=tcp:{Name}.database.windows.net;Initial Catalog={databaseName};Authentication=Active Directory Managed Identity;MultipleActiveResultSets=False;Encrypt=True;Connection Timeout=30;";

            var sqlServer = new Server(Name, new ServerArgs
            {
                ServerName = Name,
                ResourceGroupName = ResourceGroupName,
                Version = "12.0",
                AdministratorLogin = username,
                AdministratorLoginPassword = _password,
                Administrators = new ServerExternalAdministratorArgs
                {
                    AdministratorType = AdministratorType.ActiveDirectory,
                    TenantId = projectConfig.Require("tenantId"),
                    Sid = _currentUserObjectId,
                    AzureADOnlyAuthentication = true,
                    Login = "SqlServerAdmin",
                    PrincipalType = PrincipalType.User,
                },
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
