param (
    [string] [Parameter(Mandatory = $true)] $environment
)

$rootDirectory = "$PSScriptRoot/../../../"

$pulumiOutputs = (Get-Content "$rootDirectory/src/Infrastructure/outputs.json" | ConvertFrom-Json)

$ip = (Invoke-RestMethod http://ipinfo.io/json | Select-Object -exp ip)
az sql server firewall-rule create -g "webapi-$environment-euw-rg" -s "webapi-$environment-euw-sqls" -n "BuildAgent" --start-ip-address $ip --end-ip-address $ip

$sql = @"
DROP USER IF EXISTS [webapi-$environment-euw-app];
CREATE USER [webapi-$environment-euw-app] WITH default_schema = [dbo] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [webapi-$environment-euw-app];
ALTER ROLE db_datawriter ADD MEMBER [webapi-$environment-euw-app];
"@

sqlcmd -I -G -S "webapi-$environment-euw-sqls.database.windows.net" -d "webapi-$environment-euw-sqld" -Q $sql

az sql server firewall-rule delete -g "webapi-$environment-euw-rg" -s "webapi-$environment-euw-sqls" -n "BuildAgent"
