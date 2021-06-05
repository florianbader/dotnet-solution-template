param (
    [string] [Parameter(Mandatory = $true)] $environment
)

$rootDirectory = "$PSScriptRoot/../../../"

$pulumiOutputs = (Get-Content "$rootDirectory/src/Infrastructure/outputs.json" | ConvertFrom-Json)

Install-Module -Name SqlServer -Force -Scope CurrentUser

$ip = (Invoke-RestMethod http://ipinfo.io/json | Select-Object -exp ip)
az sql server firewall-rule create -g "webapi-$environment-euw-rg" -s "webapi-$environment-euw-sqls" -n "BuildAgent" --start-ip-address $ip --end-ip-address $ip

$token = (Get-AzAccessToken -ResourceUrl https://database.windows.net).Token

$query = "IF NOT EXISTS(SELECT 1 FROM sys.database_principals WHERE name ='webapi-$environment-euw-app')
    BEGIN
        CREATE USER [webapi-$environment-euw-app] WITH DEFAULT_SCHEMA=[dbo], SID = $sid, TYPE = E;
    END

    IF IS_ROLEMEMBER('db_datareader','webapi-$environment-euw-app') = 0
    BEGIN
        ALTER ROLE db_datareader ADD MEMBER [webapi-$environment-euw-app]
    END

    IF IS_ROLEMEMBER('db_datawriter','webapi-$environment-euw-app') = 0
    BEGIN
        ALTER ROLE db_datawriter ADD MEMBER [webapi-$environment-euw-app]
    END;"

Invoke-SqlCmd -ServerInstance "webapi-$environment-euw-sqls" `
    -Database "webapi-$environment-euw-sqld" `
    -AccessToken "$token" `
    -Query "$query"

az sql server firewall-rule delete -g "webapi-$environment-euw-rg" -s "webapi-$environment-euw-sqls" -n "BuildAgent"
