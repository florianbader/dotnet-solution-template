param (
    [string] [Parameter(Mandatory = $true)] $environment
)

$rootDirectory = "$PSScriptRoot/../../../"

$pulumiOutputs = (Get-Content "$rootDirectory/src/Infrastructure/outputs.json" | ConvertFrom-Json)

Install-Module -Name SqlServer -Force -Scope CurrentUser

$ip = (Invoke-RestMethod http://ipinfo.io/json | Select-Object -exp ip)
Write-Host "Creating Azure SQL firewall rule for ip $ip"
az sql server firewall-rule create -g "webapi-$environment-euw-rg" -s "webapi-$environment-euw-sqls" -n "BuildAgent" --start-ip-address $ip --end-ip-address $ip

try {
    $token = (Get-AzAccessToken -ResourceUrl https://database.windows.net).Token

    Invoke-SqlCmd -ServerInstance "webapi-$environment-euw-sqls" -Database "webapi-$environment-euw-sqld" -AccessToken "$token" -Query "SELECT 1"

    $query = "IF NOT EXISTS(SELECT 1 FROM sys.database_principals WHERE name ='webapi-$environment-euw-app')
        BEGIN
            CREATE USER [webapi-$environment-euw-app] WITH DEFAULT_SCHEMA=[dbo] FROM EXTERNAL PROVIDER;
        END

        IF IS_ROLEMEMBER('db_datareader','webapi-$environment-euw-app') = 0
        BEGIN
            ALTER ROLE db_datareader ADD MEMBER [webapi-$environment-euw-app]
        END

        IF IS_ROLEMEMBER('db_datawriter','webapi-$environment-euw-app') = 0
        BEGIN
            ALTER ROLE db_datawriter ADD MEMBER [webapi-$environment-euw-app]
        END;"

    Write-Host "Adding permission for apps in Azure SQL database"
    Invoke-SqlCmd -ServerInstance "webapi-$environment-euw-sqls" -Database "webapi-$environment-euw-sqld" -AccessToken "$token" -Query "$query"
}
finally {
    Write-Host "Removing Azure SQL firewall rule"
    az sql server firewall-rule delete -g "webapi-$environment-euw-rg" -s "webapi-$environment-euw-sqls" -n "BuildAgent"
}
