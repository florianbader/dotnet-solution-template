param (
    [string] [Parameter(Mandatory = $true)] $environment,
    [string] [Parameter(Mandatory = $true)] $location
)

$rootDirectory = "$PSScriptRoot/../../../"

$pulumiOutputs = (Get-Content "$rootDirectory/src/Infrastructure/outputs.json" | ConvertFrom-Json)

$ip = (Invoke-RestMethod http://ipinfo.io/json | Select-Object -exp ip)
az sql server firewall-rule create -g "webapi-$environment-$location-rg" -s "webapi-$environment-$location-sqls" -n "BuildAgent" --start-ip-address $ip --end-ip-address $ip

$sql = @"
DROP USER IF EXISTS [webapi-$environment-$location-app];
CREATE USER [webapi-$environment-$location-app] WITH default_schema = [dbo] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [webapi-$environment-$location-app];
ALTER ROLE db_datawriter ADD MEMBER [webapi-$environment-$location-app];
"@

sqlcmd -I -G -S "webapi-$environment-$location-sqls.database.windows.net" -d "webapi-$environment-$location-sqld" -Q $sql

az sql server firewall-rule delete -g "webapi-$environment-$location-rg" -s "webapi-$environment-$location-sqls" -n "BuildAgent"
